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
        #region Fields & Properties
        private readonly TrayApplicationContext _trayApplicationContext;
        private bool _useRegexSearch;
        private bool _caseSensitiveSearch;
        public bool CaseSensitive => _caseSensitiveSearch;
        public bool IsLocked { get; private set; }
        #endregion

        #region Constructor
        public MainFormViewModel(TrayApplicationContext trayApplicationContext)
        {
            _trayApplicationContext = trayApplicationContext;
        }
        #endregion

        #region Search & Filtering
        public void SetSearchOptions(bool useRegex, bool caseSensitive)
        {
            _useRegexSearch = useRegex;
            _caseSensitiveSearch = caseSensitive;
        }
        private IEnumerable<ClipboardItem> GetFilteredItems(string searchTerm)
        {
            var cache = _trayApplicationContext.GetClipboardCache();
            if (string.IsNullOrWhiteSpace(searchTerm)) return cache;
            if (_useRegexSearch)
            {
                try
                {
                    var options = _caseSensitiveSearch
                        ? RegexOptions.None
                        : RegexOptions.IgnoreCase;
                    var regex = new Regex(searchTerm, options);
                    return cache.Where(i => i.Content != null && regex.IsMatch(i.Content));
                }
                catch
                {
                    return Enumerable.Empty<ClipboardItem>();
                }
            }
            var comparison = _caseSensitiveSearch
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;
            return cache.Where(i =>
                i.Content != null &&
                i.Content.IndexOf(searchTerm, comparison) >= 0);
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
                return pinned
                    .OrderByDescending(i => i.Timestamp)
                    .Concat(unpinned.OrderByDescending(i => i.Timestamp));
            }
            return unpinned
                .OrderBy(i => i.Timestamp)
                .Concat(pinned.OrderBy(i => i.Timestamp));
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
        #endregion

        #region Clipboard Copy Operations
        public void CopyClicked(ClipboardItem selectedItem, bool asObject)
        {
            if (selectedItem == null) return;
            if (SettingsLoader.Current.SuppressClipboardEvents)
                _trayApplicationContext.SuppressClipboardEvents(true);
            try
            {
                DataObject dataObj = new DataObject();
                if (selectedItem.ItemType == ClipboardItemType.Image && selectedItem.ImageContent != null)
                {
                    dataObj.SetData(DataFormats.Bitmap, true, selectedItem.ImageContent);
                    try
                    {
                        string tempPath = CreatePathForCopy(selectedItem);
                        selectedItem.ImageContent.Save(tempPath, ImageFormat.Png);

                        var fileList = new System.Collections.Specialized.StringCollection { tempPath };
                        dataObj.SetFileDropList(fileList);
                    }
                    catch (Exception) { /* Log: ex.Message */ }

                    Clipboard.SetDataObject(dataObj, true);
                }
                else if (selectedItem.ItemType == ClipboardItemType.Path)
                {
                    var fileList = new System.Collections.Specialized.StringCollection { selectedItem.Content };
                    Clipboard.SetFileDropList(fileList);
                }
                else if (!string.IsNullOrEmpty(selectedItem.Content))
                {
                    if (asObject)
                    {
                        dataObj.SetData(DataFormats.UnicodeText, true, selectedItem.Content);
                        try
                        {
                            string tempPath = CreatePathForCopy(selectedItem);
                            File.WriteAllText(tempPath, selectedItem.Content, Encoding.UTF8);
                            var fileList = new System.Collections.Specialized.StringCollection { tempPath };
                            dataObj.SetFileDropList(fileList);
                        }
                        catch (Exception) { /* Log: ex.Message */ }
                        Clipboard.SetDataObject(dataObj, true);
                    }
                    else
                    {
                        Clipboard.SetText(selectedItem.Content, TextDataFormat.UnicodeText);
                    }
                }
            }
            finally
            {
                if (SettingsLoader.Current.SuppressClipboardEvents)
                {
                    Task.Delay(150).ContinueWith(_ =>
                        _trayApplicationContext.SuppressClipboardEvents(false)
                    );
                }
            }
        }
        public string CreatePathForCopy(ClipboardItem item, bool isSaveOperation = false)
        {
            string fileName = FileOpener.GetUnifiedFileName(item);
            if (isSaveOperation)
            {
                return fileName;
            }
            return Path.Combine(Path.GetTempPath(), fileName);
        }
        #endregion

        #region Pin & Item State Management
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
        public void DeleteItem(ClipboardItem item)
        {
            if (item == null) return;
            if (item.IsPinned)
            {
                TempConfigLoader.Current.PinnedHashes.Remove(item.Id);
                TempConfigLoader.Save();
            }
            _trayApplicationContext.RequestDeletion(item);
        }
        #endregion

        #region File & Disk Operations
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
            else if (item.ItemType == ClipboardItemType.Path)
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
        public SaveFileInfo GetSaveFileInfo(ClipboardItem item)
        {
            var title = FileOpener.GetUnifiedFileName(item);

            var info = new SaveFileInfo();
            switch (item.ItemType)
            {
                case ClipboardItemType.Image:
                    info.Title = "Save Image";
                    info.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                    info.FileName = title;
                    break;
                case ClipboardItemType.Path:
                    info.Title = "Save File As";
                    info.FileName = !string.IsNullOrEmpty(item.Content) ? Path.GetFileName(item.Content) : "unknown_file";
                    info.Filter = "All files|*.*";
                    break;
                default:
                    info.Title = "Save Text";
                    info.Filter = "Text File|*.txt|All files|*.*";
                    info.FileName = title;
                    break;
            }
            return info;
        }
        #endregion

        #region Index & Ordering Logic
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
            return GetInsertionIndex(item, items.Count, (i) => items[i] as ClipboardItem);
        }
        public int GetIndexToRemove(IEnumerable<ClipboardItem> currentItems)
        {
            var items = currentItems.ToList();
            if (items.Count == 0) return -1;

            if (SettingsLoader.Current.InvertClipboardHistoryListing)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (!items[i].IsPinned) return i;
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i].IsPinned) return i;
                }
            }
            return -1;
        }
        #endregion

        #region Validation & Helpers
        public string GetStatusText()
        {
            int memory = _trayApplicationContext.GetClipboardCache().Count;
            int stored = _trayApplicationContext.GetStoredCount();
            return $"Memory: {memory} | Stored: {stored}";
        }
        public bool ToggleLock(bool alwaysTopMost)
        {
            IsLocked = !IsLocked;
            return IsLocked || alwaysTopMost;
        }
        #endregion

        public MenuState GetMenuState(ClipboardItem item)
        {
            if (item == null) return null;

            bool isFile = File.Exists(item.Content);
            bool isDir = Directory.Exists(item.Content);
            bool isPathFound = isFile || isDir;

            bool canCopy = item.ItemType != ClipboardItemType.Path || isPathFound;
            bool canSave = item.ItemType != ClipboardItemType.Path || isFile;

            bool canOpen = !(item.ItemType == ClipboardItemType.Path && !isPathFound);

            return new MenuState
            {
                CanCopy = canCopy,
                CanOpen = canOpen,
                CanSave = canSave,
                IsPinned = item.IsPinned,
                PinText = item.IsPinned ? "Unpin" : "Pin"
            };
        }

        public void Dispose()
        {
            // Clean up resources if necessary
        }
    }
}