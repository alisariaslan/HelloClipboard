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
					if (!string.IsNullOrEmpty(item.Id) && TempConfigLoader.Current.PinnedHashes.Contains(item.Id))
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

		private DateTime _lastCaptureTime = DateTime.MinValue;

		private void AddToCache(ClipboardItemType type, string textContent, Image imageContent = null)
		{
			if (string.IsNullOrWhiteSpace(textContent) && imageContent == null) return;

			var now = DateTime.Now;
			var timeDiff = (now - _lastCaptureTime).TotalMilliseconds;

			string imageHash = (type == ClipboardItemType.Image && imageContent != null)
				? HashHelper.HashImageBytes(imageContent)
				: null;

			bool isSameAsLast = (type == ClipboardItemType.Text && textContent == _lastTextContent) ||
								(type == ClipboardItemType.File && textContent == _lastFileContent) ||
								(type == ClipboardItemType.Image && !string.IsNullOrEmpty(imageHash) && imageHash == _lastImageHash);

			if (isSameAsLast && timeDiff < 500) return;

			string calculatedHash = (type == ClipboardItemType.Image) ? imageHash : HashHelper.CalculateMd5Hash(textContent);
			bool preventDuplication = SettingsLoader.Current.PreventClipboardDuplication;

			if (preventDuplication && _clipboardHashPool.Contains(calculatedHash))
			{
				var existingItem = _clipboardCache.FirstOrDefault(i => i.ContentHash == calculatedHash);
				if (existingItem != null)
				{
					string oldId = existingItem.Id; 
					_historyHelper.DeleteItemFromFile(oldId);

					existingItem.Timestamp = now;
					existingItem.Id = now.Ticks.ToString() + "_" + calculatedHash; 

					if (existingItem.IsPinned)
					{
						TempConfigLoader.Current.PinnedHashes.Remove(oldId);
						TempConfigLoader.Current.PinnedHashes.Add(existingItem.Id);
						TempConfigLoader.Save();
					}

					_clipboardCache.Remove(existingItem);
					_clipboardCache.Add(existingItem);

					if (SettingsLoader.Current.EnableClipboardHistory)
						Task.Run(() => _historyHelper.SaveItemToHistoryFile(existingItem));

					UpdateLastCaptureInfo(type, textContent, imageHash, now);
					ItemUpdated?.Invoke(existingItem);
					return;
				}
			}

			var item = new ClipboardItem(type, textContent, GenerateTitle(type, textContent), imageContent, calculatedHash);

			if (item.ContentHash != null && TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
				item.IsPinned = true;

			if (SettingsLoader.Current.EnableClipboardHistory && item.ContentHash != null)
				Task.Run(() => _historyHelper.SaveItemToHistoryFile(item));

			_clipboardCache.Add(item);
			_clipboardHashPool.Add(calculatedHash);

			UpdateLastCaptureInfo(type, textContent, imageHash, now);

			ItemCaptured?.Invoke(item);
			CheckHistoryLimit();
		}

		private void UpdateLastCaptureInfo(ClipboardItemType type, string text, string imgHash, DateTime captureTime)
		{
			if (type == ClipboardItemType.Text) _lastTextContent = text;
			else if (type == ClipboardItemType.File) _lastFileContent = text;
			else if (type == ClipboardItemType.Image) _lastImageHash = imgHash;

			_lastCaptureTime = captureTime; // Zamanı kaydet
		}

		private void CheckHistoryLimit()
		{
			// Sabitlenmemiş (IsPinned = false) öğelerin sayısını kontrol et
			// Pinned öğeler bu sınıra dahil değildir ve asla silinmez.
			while (_clipboardCache.Count(i => !i.IsPinned) > SettingsLoader.Current.MaxHistoryCount)
			{
				// Sabitlenmemişler arasında en eski (Timestamp'i en küçük) olanı bul
				var oldestUnpinned = _clipboardCache
					.Where(i => !i.IsPinned)
					.OrderBy(i => i.Timestamp)
					.FirstOrDefault();

				if (oldestUnpinned != null)
				{
					// 1. ADIM: Diskten sil (Id üzerinden)
					_historyHelper.DeleteItemFromFile(oldestUnpinned.Id);

					// 2. ADIM: RAM'deki Cache'den sil
					_clipboardCache.Remove(oldestUnpinned);

					// 3. ADIM: Hash havuzundan (duplication check) temizle 
					// (Eğer aynı içeriğe sahip başka bir öğe kalmadıysa)
					if (!_clipboardCache.Any(i => i.ContentHash == oldestUnpinned.ContentHash))
					{
						_clipboardHashPool.Remove(oldestUnpinned.ContentHash);
					}

					// 4. ADIM: UI'ya öğenin silindiğini bildir (ListBox güncellemesi için)
					ItemRemoved?.Invoke();
				}
				else
				{
					// Silecek unpinned öğe kalmadıysa döngüden çık
					break;
				}
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
			_historyHelper.ClearAllHistoryFiles();
			ClipboardCleared?.Invoke();
		}

		public IReadOnlyList<ClipboardItem> GetCache() => _clipboardCache.AsReadOnly();
	}
}