using HelloClipboard.Models;
using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class MainFormViewModel : IDisposable
	{
		private readonly TrayApplicationContext _trayApplicationContext;

		private bool _useRegexSearch;
		private bool _caseSensitiveSearch;

		// Properties the Form needs to reference while rendering/drawing
		public bool CaseSensitive => _caseSensitiveSearch;

		public MainFormViewModel(TrayApplicationContext trayApplicationContext)
		{
			_trayApplicationContext = trayApplicationContext;
		}

		/// <summary>
		/// Configures the search behavior.
		/// </summary>
		public void SetSearchOptions(bool useRegex, bool caseSensitive)
		{
			_useRegexSearch = useRegex;
			_caseSensitiveSearch = caseSensitive;
		}

		/// <summary>
		/// Returns a filtered list of clipboard items based on the search term.
		/// </summary>
		public IEnumerable<ClipboardItem> GetFilteredItems(string searchTerm)
		{
			var cache = _trayApplicationContext.GetClipboardCache();
			if (string.IsNullOrWhiteSpace(searchTerm)) return cache;

			if (_useRegexSearch)
			{
				try
				{
					var options = _caseSensitiveSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
					var regex = new Regex(searchTerm, options);
					return cache.Where(i => i.Content != null && regex.IsMatch(i.Content));
				}
				catch { return Enumerable.Empty<ClipboardItem>(); }
			}

			var comparison = _caseSensitiveSearch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			return cache.Where(i => i.Content != null && i.Content.IndexOf(searchTerm, comparison) >= 0);
		}

		/// <summary>
		/// Generates a Regex object to be used for text highlighting in the UI.
		/// </summary>
		public Regex GetHighlightRegex(string searchTerm)
		{
			if (!_useRegexSearch || string.IsNullOrWhiteSpace(searchTerm)) return null;
			try
			{
				return new Regex(searchTerm, _caseSensitiveSearch ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			catch { return null; }
		}

		/// <summary>
		/// Handles the logic when an item is selected to be copied back to the system clipboard.
		/// </summary>
		public void CopyClicked(ClipboardItem selectedItem)
		{
			if (selectedItem == null) return;

			// Suppress monitor events to prevent re-capturing the same item
			_trayApplicationContext.SuppressClipboardEvents(true);
			try
			{
				if (selectedItem.ItemType == ClipboardItemType.Image)
					Clipboard.SetImage(selectedItem.ImageContent);
				else if (!string.IsNullOrEmpty(selectedItem.Content))
					Clipboard.SetText(selectedItem.Content);
			}
			finally
			{
				// Brief delay before re-enabling monitor to ensure OS clipboard operations complete
				Task.Delay(150).ContinueWith(_ => _trayApplicationContext.SuppressClipboardEvents(false));
			}
		}

		/// <summary>
		/// Toggles the pinned status of an item and persists the change to configuration.
		/// </summary>
		public bool TogglePin(ClipboardItem item)
		{
			if (item == null || string.IsNullOrEmpty(item.Id)) return false;

			item.IsPinned = !item.IsPinned;

			if (item.IsPinned)
			{
				if (!TempConfigLoader.Current.PinnedHashes.Contains(item.Id))
					TempConfigLoader.Current.PinnedHashes.Add(item.Id);
			}
			else
			{
				TempConfigLoader.Current.PinnedHashes.Remove(item.Id);
			}

			TempConfigLoader.Save();
			return item.IsPinned;
		}

		/// <summary>
		/// Saves the clipboard item content (Image, File, or Text) to a specific disk location.
		/// </summary>
		public void SaveItemToDisk(ClipboardItem item, string targetPath)
		{
			if (item == null || string.IsNullOrEmpty(targetPath)) return;

			if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
			{
				var ext = Path.GetExtension(targetPath)?.ToLowerInvariant();
				var format = ImageFormat.Png; // Default format
				if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
				else if (ext == ".bmp") format = ImageFormat.Bmp;

				item.ImageContent.Save(targetPath, format);
			}
			else if (item.ItemType == ClipboardItemType.File)
			{
				if (File.Exists(item.Content))
				{
					File.Copy(item.Content, targetPath, true);
				}
			}
			else // Text items
			{
				File.WriteAllText(targetPath, item.Content ?? string.Empty, Encoding.UTF8);
			}
		}

		/// <summary>
		/// Calculates the correct UI insertion index based on pinning and sort order settings.
		/// </summary>
		public int GetInsertionIndex(ClipboardItem item, int currentItemCount, Func<int, ClipboardItem> getItemAt)
		{
			bool invert = SettingsLoader.Current.InvertClipboardHistoryListing;

			if (item.IsPinned)
			{
				if (invert) return 0; 

				return currentItemCount;
			}

			int pinnedCount = 0;
			List<ClipboardItem> unpinnedItems = new List<ClipboardItem>();

			for (int i = 0; i < currentItemCount; i++)
			{
				var current = getItemAt(i);
				if (current == null) continue;

				if (current.IsPinned) pinnedCount++;
				else unpinnedItems.Add(current);
			}

			int indexInUnpinned = 0;
			foreach (var existingUnpinned in unpinnedItems)
			{
				if (invert)
				{
					if (item.Timestamp > existingUnpinned.Timestamp) break;
				}
				else
				{
					if (item.Timestamp < existingUnpinned.Timestamp) break;
				}
				indexInUnpinned++;
			}

			if (invert)
			{
				return pinnedCount + indexInUnpinned;
			}
			else
			{
				return indexInUnpinned;
			}
		}

		public int GetVisualInsertionIndex(ClipboardItem item, ListBox.ObjectCollection items)
		{
			// Pin'leme işlemi değiştiğinde elemanın yeni yerini bulur
			return GetInsertionIndex(item, items.Count, (i) => items[i] as ClipboardItem);
		}


		// 1 & 3. Index Calculation for Deletion and Insertion
		/// <summary>
		/// Determines which unpinned item should be removed when the history limit is reached.
		/// </summary>
		public int GetIndexToRemove(IEnumerable<ClipboardItem> currentItems)
		{
			var items = currentItems.ToList();
			if (items.Count == 0) return -1;

			if (SettingsLoader.Current.InvertClipboardHistoryListing)
			{
				// For inverted listing (newest on top), find the last (oldest) unpinned item
				for (int i = items.Count - 1; i >= 0; i--)
				{
					if (!items[i].IsPinned) return i;
				}
			}
			else
			{
				// For normal listing, find the first unpinned item
				for (int i = 0; i < items.Count; i++)
				{
					if (!items[i].IsPinned) return i;
				}
			}
			return -1;
		}

		// 2. URL and Menu State Validation
		public bool IsValidUrl(string content)
		{
			return !string.IsNullOrWhiteSpace(content) && UrlHelper.IsValidUrl(content);
		}

		// 5. SaveDialog Filename and Filter Generation
		/// <summary>
		/// Prepares metadata for the Save File Dialog based on the item type.
		/// </summary>
		public SaveFileInfo GetSaveFileInfo(ClipboardItem item)
		{
			var info = new SaveFileInfo();
			switch (item.ItemType)
			{
				case ClipboardItemType.Image:
					info.Title = "Save Image";
					info.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
					info.FileName = "clipboard_image";
					break;
				case ClipboardItemType.File:
					info.Title = "Save File As";
					info.FileName = Path.GetFileName(item.Content);
					info.Filter = "All files|*.*";
					break;
				default:
					info.Title = "Save Text";
					info.Filter = "Text File|*.txt|All files|*.*";
					info.FileName = "clipboard_text.txt";
					break;
			}
			return info;
		}

		public IEnumerable<ClipboardItem> GetDisplayList(string searchTerm = "")
		{
			var cache = _trayApplicationContext.GetClipboardCache();
			bool invert = SettingsLoader.Current.InvertClipboardHistoryListing;

			var filtered = string.IsNullOrWhiteSpace(searchTerm)
				? cache
				: GetFilteredItems(searchTerm);

			var pinned = filtered.Where(i => i.IsPinned);
			var unpinned = filtered.Where(i => !i.IsPinned);

			if (invert)
			{
				// [Pinned] -> [Unpinned]
				var pinnedSorted = pinned.OrderByDescending(i => i.Timestamp);
				var unpinnedSorted = unpinned.OrderByDescending(i => i.Timestamp);
				return pinnedSorted.Concat(unpinnedSorted);
			}
			else
			{
				//  [Unpinned] -> [Pinned]
				var pinnedSorted = pinned.OrderBy(i => i.Timestamp);
				var unpinnedSorted = unpinned.OrderBy(i => i.Timestamp);
				return unpinnedSorted.Concat(pinnedSorted);
			}
		}

		public void Dispose()
		{
			// Clean up resources if necessary
		}
	}
}