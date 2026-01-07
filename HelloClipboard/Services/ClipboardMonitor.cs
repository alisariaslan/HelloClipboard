using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
	public class ClipboardMonitor
	{
		private readonly List<ClipboardItem> _clipboardCache = new List<ClipboardItem>();
		private readonly HashSet<string> _clipboardHashPool = new HashSet<string>();

		private uint _lastClipboardSequenceNumber;
		private string _lastTextContent;
		private string _lastFileContent;
		private string _lastImageHash;
		private bool _suppressClipboardEvents = false;

		private const int ClipboardMaxAttempts = 4;
		private const int ClipboardFastRetryDelayMs = 25;

		private readonly HistoryHelper _historyHelper;
		private readonly PrivacyService _privacyService;

		// Events to notify the UI when items are added, updated, or cleared
		public event Action<ClipboardItem> ItemCaptured;
		public event Action<ClipboardItem> ItemUpdated; // Triggered for re-ordering on duplication
		public event Action ItemRemoved; // Triggered when the oldest item is removed due to capacity limit
		public event Action ClipboardCleared;

		[DllImport("user32.dll")]
		private static extern uint GetClipboardSequenceNumber();

		public ClipboardMonitor(HistoryHelper historyHelper, PrivacyService privacyService)
		{
			_historyHelper = historyHelper;
			_privacyService = privacyService;

			// Load saved history on initialization
			LoadInitialHistory();
		}

		private void LoadInitialHistory()
		{
			if (SettingsLoader.Current.EnableClipboardHistory)
			{
				var loadedItems = _historyHelper.LoadHistoryFromFiles();
				foreach (var item in loadedItems)
				{
					if (item.ContentHash != null && TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
					{
						item.IsPinned = true;
					}
					_clipboardCache.Add(item);
					if (item.ContentHash != null) _clipboardHashPool.Add(item.ContentHash);
				}
			}
		}

		public void Start()
		{
			ClipboardNotification.ClipboardUpdate += OnClipboardUpdate;
		}

		public void SuppressEvents(bool suppress) => _suppressClipboardEvents = suppress;

		private async void OnClipboardUpdate(object sender, EventArgs e)
		{
			if (_suppressClipboardEvents || _privacyService.IsActive)
				return;

			uint seq = GetClipboardSequenceNumber();
			if (seq != 0 && seq == _lastClipboardSequenceNumber)
				return;

			_lastClipboardSequenceNumber = seq;
			await TryReadClipboardAsync();
		}

		private async Task TryReadClipboardAsync()
		{
			for (int attempt = 0; attempt < ClipboardMaxAttempts; attempt++)
			{
				try
				{
					var dataObj = Clipboard.GetDataObject();
					if (dataObj == null) return;

					// Handle TEXT content
					if (dataObj.GetDataPresent(DataFormats.UnicodeText, true))
					{
						if (dataObj.GetData(DataFormats.UnicodeText, true) is string text && !string.IsNullOrEmpty(text))
						{
							AddToCache(ClipboardItemType.Text, text);
							return;
						}
					}

					// Handle FILE content
					if (dataObj.GetDataPresent(DataFormats.FileDrop))
					{
						if (dataObj.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
						{
							foreach (var file in files) AddToCache(ClipboardItemType.File, file);
							return;
						}
					}

					// Handle IMAGE content
					if (dataObj.GetDataPresent(DataFormats.Bitmap))
					{
						var image = Clipboard.GetImage();
						if (image != null)
						{
							var imageCount = _clipboardCache.Count(i => i.ItemType == ClipboardItemType.Image);
							AddToCache(ClipboardItemType.Image, $"[IMAGE {imageCount + 1}]", image);
							return;
						}
					}

					if (attempt < ClipboardMaxAttempts - 1) await Task.Delay(ClipboardFastRetryDelayMs);
				}
				catch (Exception)
				{
					if (attempt < ClipboardMaxAttempts - 1) await Task.Delay(ClipboardFastRetryDelayMs);
				}
			}
		}

		private void AddToCache(ClipboardItemType type, string textContent, Image imageContent = null)
		{
			if (string.IsNullOrWhiteSpace(textContent) && imageContent == null) return;

			bool preventDuplication = SettingsLoader.Current.PreventClipboardDuplication;

			// Quick duplication check against last known content
			if (preventDuplication)
			{
				if (type == ClipboardItemType.Text && textContent == _lastTextContent) return;
				if (type == ClipboardItemType.File && textContent == _lastFileContent) return;
			}

			string imageHash = (type == ClipboardItemType.Image && imageContent != null) ? HashHelper.HashImageBytes(imageContent) : null;
			if (preventDuplication && type == ClipboardItemType.Image && !string.IsNullOrEmpty(imageHash) && imageHash == _lastImageHash) return;

			// Calculate Hash for comparison
			string calculatedHash = null;
			if (preventDuplication)
			{
				calculatedHash = (type == ClipboardItemType.Image) ? imageHash : HashHelper.CalculateMd5Hash(textContent);
			}

			// If item already exists in pool (previously copied), move it to the top
			if (calculatedHash != null && _clipboardHashPool.Contains(calculatedHash))
			{
				var existingItem = _clipboardCache.FirstOrDefault(i => i.ContentHash == calculatedHash);
				if (existingItem != null)
				{
					_clipboardCache.Remove(existingItem);
					_clipboardCache.Add(existingItem);
					ItemUpdated?.Invoke(existingItem);
				}
				return;
			}

			// Generate a display title for the item
			string newTitle = GenerateTitle(type, textContent);
			var item = new ClipboardItem(_clipboardCache.Count, type, textContent, newTitle, imageContent, calculatedHash);

			if (item.ContentHash != null && TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
				item.IsPinned = true;

			// Save to history storage
			if (SettingsLoader.Current.EnableClipboardHistory && item.ContentHash != null)
				Task.Run(() => _historyHelper.SaveItemToHistoryFile(item));

			// Add to cache with pinning logic
			InsertToCache(item);

			// Update state with last captured values
			if (type == ClipboardItemType.Text) _lastTextContent = textContent;
			else if (type == ClipboardItemType.File) _lastFileContent = textContent;
			else if (type == ClipboardItemType.Image) _lastImageHash = imageHash;

			ItemCaptured?.Invoke(item);
			CheckHistoryLimit();
		}

		private void InsertToCache(ClipboardItem item)
		{
			if (item.IsPinned)
			{
				_clipboardCache.Add(item);
			}
			else
			{
				// New unpinned items are inserted after the block of pinned items
				var insertIndex = _clipboardCache.TakeWhile(i => i.IsPinned).Count();
				_clipboardCache.Insert(insertIndex, item);
			}

			if (item.ContentHash != null) _clipboardHashPool.Add(item.ContentHash);
		}

		private void CheckHistoryLimit()
		{
			if (_clipboardCache.Count > SettingsLoader.Current.MaxHistoryCount)
			{
				// Find the oldest item that isn't pinned; fall back to the very first item if all are pinned
				var oldestItem = _clipboardCache.FirstOrDefault(i => !i.IsPinned) ?? _clipboardCache[0];

				if (oldestItem.ContentHash != null)
				{
					_clipboardHashPool.Remove(oldestItem.ContentHash);
					if (SettingsLoader.Current.EnableClipboardHistory)
						_historyHelper.DeleteItemFromFile(oldestItem.ContentHash);
				}

				_clipboardCache.Remove(oldestItem);
				ItemRemoved?.Invoke();
			}
		}

		private string GenerateTitle(ClipboardItemType type, string content)
		{
			if (type == ClipboardItemType.Text)
			{
				// Sanitize text by removing newlines and tabs for UI display
				string cleaned = Regex.Replace(content.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' '), @"\s+", " ").Trim();
				return cleaned.Length > 1024 ? cleaned.Substring(0, 1024) + "..." : cleaned;
			}
			if (type == ClipboardItemType.File) return $"{Path.GetFileName(content)} -> {content}";
			return content; // Return content as is for Image titles
		}

		public void ClearAll()
		{
			_clipboardCache.Clear();
			_clipboardHashPool.Clear();
			ClipboardCleared?.Invoke();
		}

		public IReadOnlyList<ClipboardItem> GetCache() => _clipboardCache.AsReadOnly();
	}
}