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

		// Form'un çizim yaparken bilmesi gereken özellikler
		public bool CaseSensitive => _caseSensitiveSearch;

		public MainFormViewModel(TrayApplicationContext trayApplicationContext)
		{
			_trayApplicationContext = trayApplicationContext;
		}

		public void LoadSettings() => SettingsLoader.LoadSettings();

		public void SetSearchOptions(bool useRegex, bool caseSensitive)
		{
			_useRegexSearch = useRegex;
			_caseSensitiveSearch = caseSensitive;
		}

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

		public Regex GetHighlightRegex(string searchTerm)
		{
			if (!_useRegexSearch || string.IsNullOrWhiteSpace(searchTerm)) return null;
			try
			{
				return new Regex(searchTerm, _caseSensitiveSearch ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			catch { return null; }
		}

		public void CopyClicked(ClipboardItem selectedItem)
		{
			if (selectedItem == null) return;
			_trayApplicationContext.SuppressClipboardEvents(true);
			try
			{
				if (selectedItem.ItemType == ClipboardItemType.Image) Clipboard.SetImage(selectedItem.ImageContent);
				else if (!string.IsNullOrEmpty(selectedItem.Content)) Clipboard.SetText(selectedItem.Content);
			}
			finally
			{
				Task.Delay(150).ContinueWith(_ => _trayApplicationContext.SuppressClipboardEvents(false));
			}
		}

		public bool TogglePin(ClipboardItem item)
		{
			if (item == null || item.ContentHash == null) return false;
			item.IsPinned = !item.IsPinned;
			if (item.IsPinned)
			{
				if (!TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
					TempConfigLoader.Current.PinnedHashes.Add(item.ContentHash);
			}
			else TempConfigLoader.Current.PinnedHashes.Remove(item.ContentHash);

			TempConfigLoader.Save();
			return item.IsPinned;
		}

		public void SaveItemToDisk(ClipboardItem item, string targetPath)
		{
			if (item == null || string.IsNullOrEmpty(targetPath)) return;

			if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
			{
				var ext = Path.GetExtension(targetPath)?.ToLowerInvariant();
				var format = ImageFormat.Png; // Varsayılan
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
			else // Text
			{
				File.WriteAllText(targetPath, item.Content ?? string.Empty, Encoding.UTF8);
			}
		}

		public int GetInsertionIndex(ClipboardItem item, int currentItemCount, Func<int, ClipboardItem> getItemAt)
		{
			bool invert = SettingsLoader.Current.InvertClipboardHistoryListing;

			if (invert)
			{
				if (item.IsPinned) return 0; // En başa

				// Pinlenmiş öğelerin hemen arkasına ekle
				int offset = 0;
				for (int i = 0; i < currentItemCount; i++)
				{
					if (getItemAt(i)?.IsPinned == true) offset++;
					else break;
				}
				return offset;
			}
			else
			{
				if (item.IsPinned) return 0;

				int offset = 0;
				for (int i = 0; i < currentItemCount; i++)
				{
					if (getItemAt(i)?.IsPinned == true) offset++;
					else break;
				}
				return offset;
			}
		}

		

		// 1 & 3. Silme ve Ekleme Index Hesabı
		public int GetIndexToRemove(IEnumerable<ClipboardItem> currentItems)
		{
			var items = currentItems.ToList();
			if (items.Count == 0) return -1;

			if (SettingsLoader.Current.InvertClipboardHistoryListing)
			{
				// Tersten listelemede (yeni en üstte), en alttaki unpinned öğeyi bul
				for (int i = items.Count - 1; i >= 0; i--)
				{
					if (!items[i].IsPinned) return i;
				}
			}
			else
			{
				// Normal listelemede ilk unpinned öğeyi bul
				for (int i = 0; i < items.Count; i++)
				{
					if (!items[i].IsPinned) return i;
				}
			}
			return -1;
		}

		// 2. URL ve Menu Durum Kontrolü
		public bool IsValidUrl(string content)
		{
			return !string.IsNullOrWhiteSpace(content) && UrlHelper.IsValidUrl(content);
		}

		// 5. SaveDialog Dosya Adı ve Filtre Üretme
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

		public void Dispose()
		{
			// Empty for now
		}
	}
}