using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HelloClipboard.Utils;

namespace HelloClipboard
{
	public class HistoryHelper
	{
		private readonly int _maxHistoryCount; 

		public HistoryHelper()
		{
			_maxHistoryCount = SettingsLoader.Current.MaxHistoryCount;
		}

		public void SaveItemToHistoryFile(ClipboardItem item)
		{
			if (item.ContentHash == null)
				return;

			string historyDir = Constants.HistoryDirectory;
			try
			{
				if (!Directory.Exists(historyDir))
				{
					Directory.CreateDirectory(historyDir);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Error creating history directory: {ex.Message}");
#endif
				return;
			}

			string extension = FileExtensionHelper.GetFileExtension(item.ItemType);
			string filePath = Path.Combine(historyDir, item.ContentHash + extension);

			if (File.Exists(filePath))
				return;

			try
			{
				if (item.ItemType == ClipboardItemType.Text || item.ItemType == ClipboardItemType.File)
				{
					File.WriteAllText(filePath, item.Content);
				}
				else if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
				{
					item.ImageContent.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Error saving clipboard item to file: {ex.Message}");
#endif
			}
		}

		public void DeleteItemFromFile(string hash)
		{
			if (string.IsNullOrWhiteSpace(hash))
				return;

			string historyDir = Constants.HistoryDirectory;
			try
			{
				var filesToDelete = Directory.GetFiles(historyDir, hash + ".*");
				foreach (var file in filesToDelete)
				{
					File.Delete(file);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Error deleting clipboard history file: {ex.Message}");
#endif
			}
		}

		public List<ClipboardItem> LoadHistoryFromFiles()
		{
			var loadedCache = new List<ClipboardItem>();
			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir))
				return loadedCache;

			var files = Directory.GetFiles(historyDir)
								 .Select(f => new FileInfo(f))
								 .OrderByDescending(f => f.LastWriteTime)
								 .ToList();

			int count = 0;
			foreach (var fileInfo in files)
			{
				if (count >= _maxHistoryCount)
				{
					try { File.Delete(fileInfo.FullName); } catch { }
					continue;
				}

				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
				string extension = Path.GetExtension(fileInfo.Name);

				ClipboardItemType type = FileExtensionHelper.GetItemTypeFromExtension(extension);

				try
				{
					string content = null;
					Image imageContent = null;
					string newTitle = "";

					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
					{
						content = File.ReadAllText(fileInfo.FullName);

						if (SettingsLoader.Current.EnableBetterHistoryVisualization && type == ClipboardItemType.Text)
						{
							string replacedNewlines = content.Replace('\r', ' ')
													 .Replace('\n', ' ')
													 .Replace('\t', ' ');
							string cleanedWhitespace = Regex.Replace(replacedNewlines, @"\s+", " ");
							newTitle = cleanedWhitespace.Trim();
							if (newTitle.Length > 1024)
							{
								newTitle = newTitle.Substring(0, 1024) + "...";
							}
						}
						else if (SettingsLoader.Current.EnableBetterHistoryVisualization && type == ClipboardItemType.File)
						{
							newTitle = $"{Path.GetFileName(content)} -> {content}";
						}
					}
					else if (type == ClipboardItemType.Image)
					{
						using (var ms = new MemoryStream(File.ReadAllBytes(fileInfo.FullName)))
						{
							imageContent = Image.FromStream(ms);
						}
						newTitle = $"[IMAGE {count}]";
					}

					if (content != null || imageContent != null)
					{
						var item = new ClipboardItem(count, type, content, newTitle, imageContent, fileNameWithoutExtension);
						loadedCache.Add(item);
						count++;
					}
				}
				catch (Exception ex)
				{
#if DEBUG
					System.Diagnostics.Debug.WriteLine($"Error loading clipboard history file {fileInfo.Name}: {ex.Message}");
#endif
					try { File.Delete(fileInfo.FullName); } catch { }
				}
			}

			if(!SettingsLoader.Current.InvertClipboardHistoryListing)
			loadedCache.Reverse();

			return loadedCache;
		}
	}
}