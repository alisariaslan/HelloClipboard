using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

			// Determine the file extension (the file will be encrypted, but we keep the extension to identify the content type)
			string extension = FileExtensionHelper.GetFileExtension(item.ItemType);
			string filePath = Path.Combine(historyDir, item.ContentHash + extension);

			if (File.Exists(filePath))
				return;

			try
			{
				byte[] rawData = null;

				if (item.ItemType == ClipboardItemType.Text || item.ItemType == ClipboardItemType.File)
				{
					// Convert text to UTF-8 byte array
					rawData = Encoding.UTF8.GetBytes(item.Content);
				}
				else if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
				{
					// Convert image to byte array using MemoryStream
					using (var ms = new MemoryStream())
					{
						item.ImageContent.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						rawData = ms.ToArray();
					}
				}

				// Encrypt the data and write it to disk
				if (rawData != null)
				{
					byte[] encryptedData = CryptoHelper.Encrypt(rawData);
					File.WriteAllBytes(filePath, encryptedData);
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
			// This method remains unchanged; the file deletion process is the same
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

			// Filter out files like 'secret.key' or use error handling to prevent loading non-history files
			var files = Directory.GetFiles(historyDir)
								 .Select(f => new FileInfo(f))
								 .OrderBy(f => f.LastWriteTime)
								 .ToList();

			int count = 0;
			int imgCount = 0;
			foreach (var fileInfo in files)
			{
				if (count >= _maxHistoryCount)
				{
					// Delete excess history files to maintain the limit
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

					// 1. Read file as binary
					byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);

					// 2. Decrypt the data
					byte[] decryptedBytes = CryptoHelper.Decrypt(fileBytes);

					if (decryptedBytes == null) continue; // Skip if decryption fails

					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
					{
						// Convert byte array back to string
						content = Encoding.UTF8.GetString(decryptedBytes);

						if (type == ClipboardItemType.Text)
						{
							// Sanitize text by replacing control characters with spaces
							string replacedNewlines = content.Replace('\r', ' ')
													 .Replace('\n', ' ')
													 .Replace('\t', ' ');
							// Collapse multiple whitespaces into a single space
							string cleanedWhitespace = Regex.Replace(replacedNewlines, @"\s+", " ");
							newTitle = cleanedWhitespace.Trim();

							if (newTitle.Length > 1024)
							{
								newTitle = newTitle.Substring(0, 1024) + "...";
							}
						}
						else if (type == ClipboardItemType.File)
						{
							newTitle = $"{Path.GetFileName(content)} -> {content}";
						}
					}
					else if (type == ClipboardItemType.Image)
					{
						// Create image from decrypted byte array
						using (var ms = new MemoryStream(decryptedBytes))
						{
							imageContent = Image.FromStream(ms);
						}
						newTitle = $"[IMAGE {++imgCount}]";
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
					// Delete corrupted files
					try { File.Delete(fileInfo.FullName); } catch { }
				}
			}

			return loadedCache;
		}
	}
}