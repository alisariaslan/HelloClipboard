using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class MainFormViewModel : IDisposable
	{
		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainForm _mainForm;
		private bool _isDisposed = false;

		// Arama ve Filtreleme State'i
		private bool _useRegexSearch;
		private bool _caseSensitiveSearch;
		private string _currentSearchTerm = string.Empty;

		public MainFormViewModel(MainForm mainForm, TrayApplicationContext trayApplicationContext)
		{
			_mainForm = mainForm;
			_trayApplicationContext = trayApplicationContext;
		}

		public void LoadSettings()
		{
			SettingsLoader.LoadSettings();
		}

		#region SEARCH & FILTERING LOGIC

		public void SetSearchOptions(bool useRegex, bool caseSensitive)
		{
			_useRegexSearch = useRegex;
			_caseSensitiveSearch = caseSensitive;
		}

		/// <summary>
		/// Arama terimine ve ayarlara göre filtrelenmiş listeyi döner.
		/// </summary>
		public IEnumerable<ClipboardItem> GetFilteredItems(string searchTerm)
		{
			_currentSearchTerm = searchTerm;
			var cache = _trayApplicationContext.GetClipboardCache();

			if (string.IsNullOrWhiteSpace(searchTerm))
				return cache;

			if (_useRegexSearch)
			{
				try
				{
					var options = _caseSensitiveSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
					var regex = new Regex(searchTerm, options);
					return cache.Where(i => i.Content != null && regex.IsMatch(i.Content));
				}
				catch
				{
					// Geçersiz Regex durumunda boş liste dön
					return Enumerable.Empty<ClipboardItem>();
				}
			}

			// Normal String Arama
			var comparison = _caseSensitiveSearch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			return cache.Where(i => i.Content != null && i.Content.IndexOf(searchTerm, comparison) >= 0);
		}

		/// <summary>
		/// UI tarafında sarı boyama (Highlight) yapmak için geçerli Regex'i döner.
		/// </summary>
		public Regex GetHighlightRegex(string searchTerm)
		{
			if (!_useRegexSearch || string.IsNullOrWhiteSpace(searchTerm))
				return null;

			try
			{
				var options = _caseSensitiveSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
				return new Regex(searchTerm, options);
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region ACTIONS (COPY, PIN, ETC.)

		public void CopyClicked(ClipboardItem selectedItem)
		{
			if (selectedItem == null) return;

			_trayApplicationContext.SuppressClipboardEvents(true);

			try
			{
				if (selectedItem.ItemType == ClipboardItemType.Image)
				{
					Clipboard.SetImage(selectedItem.ImageContent);
				}
				else if (!string.IsNullOrEmpty(selectedItem.Content))
				{
					Clipboard.SetText(selectedItem.Content);
				}
			}
			finally
			{
				// Clipboard olaylarını tekrar dinlemek için kısa bir gecikme
				Task.Delay(150).ContinueWith(_ =>
				{
					_trayApplicationContext.SuppressClipboardEvents(false);
				});
			}
		}

		/// <summary>
		/// Bir öğeyi pinler veya pini kaldırır, config'i günceller.
		/// </summary>
		public bool TogglePin(ClipboardItem item)
		{
			if (item == null || item.ContentHash == null) return false;

			item.IsPinned = !item.IsPinned;

			if (item.IsPinned)
			{
				if (!TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
					TempConfigLoader.Current.PinnedHashes.Add(item.ContentHash);
			}
			else
			{
				TempConfigLoader.Current.PinnedHashes.Remove(item.ContentHash);
			}

			TempConfigLoader.Save();
			return item.IsPinned;
		}

		#endregion

		public void Dispose()
		{
			_isDisposed = true;
		}
	}
}