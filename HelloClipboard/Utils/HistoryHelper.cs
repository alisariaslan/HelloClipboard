using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HelloClipboard
{
	public class HistoryHelper
	{
		private readonly int _maxHistoryCount;

		public HistoryHelper()
		{
			_maxHistoryCount = SettingsLoader.Current.MaxHistoryCount;
		}

		public void ClearAllHistoryFiles()
		{
			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) return;

			try
			{
				var files = Directory.GetFiles(historyDir);
				foreach (var file in files)
				{
					if (Path.GetFileName(file).Equals("secret.key", StringComparison.OrdinalIgnoreCase))
						continue;

					File.Delete(file);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error occured when cleaning history files: {ex.Message}");
			}

		}
		public void SaveItemToHistoryFile(ClipboardItem item)
		{
			if (string.IsNullOrEmpty(item.Id)) return;

			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) Directory.CreateDirectory(historyDir);

			string extension = FileExtensionHelper.GetFileExtension(item.ItemType);
		
			string filePath = Path.Combine(historyDir, item.Id + extension);

			if (File.Exists(filePath)) return;
			try
			{
				byte[] rawData = null;
				if (item.ItemType == ClipboardItemType.Text || item.ItemType == ClipboardItemType.File)
					rawData = Encoding.UTF8.GetBytes(item.Content);
				else if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
				{
					using (var ms = new MemoryStream())
					{
						item.ImageContent.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						rawData = ms.ToArray();
					}
				}

				if (rawData != null)
				{
					byte[] encryptedData = CryptoHelper.Encrypt(rawData);
					File.WriteAllBytes(filePath, encryptedData);
				}
			}
			catch (Exception) { /* Debug.WriteLine(ex.Message); */ }
		}

		public void DeleteItemFromFile(string id) 
		{
			if (string.IsNullOrWhiteSpace(id)) return;
			string historyDir = Constants.HistoryDirectory;
			try
			{
				var filesToDelete = Directory.GetFiles(historyDir, id + ".*");
				foreach (var file in filesToDelete) File.Delete(file);
			}
			catch (Exception) { /* Debug.WriteLine(ex.Message); */ }
		}

		public List<ClipboardItem> LoadHistoryFromFiles()
		{
			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) return new List<ClipboardItem>();

			EnforceDiskLimitOnDisk(historyDir);

			var loadedCache = new List<ClipboardItem>();
		

			var files = Directory.GetFiles(historyDir)
							 .Select(f => new FileInfo(f))
							 .OrderBy(f => f.LastWriteTime)
							 .ToList();

			foreach (var fileInfo in files)
			{
				if (fileInfo.Name.Equals("secret.key", StringComparison.OrdinalIgnoreCase)) continue;

				string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
				string extension = Path.GetExtension(fileInfo.Name);
				ClipboardItemType type = FileExtensionHelper.GetItemTypeFromExtension(extension);

				try
				{
					byte[] decryptedBytes = CryptoHelper.Decrypt(File.ReadAllBytes(fileInfo.FullName));
					if (decryptedBytes == null) continue;

					string content = null;
					Image imageContent = null;

					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
					{
						content = Encoding.UTF8.GetString(decryptedBytes);
					}
					else
					{
						using (var ms = new MemoryStream(decryptedBytes)) imageContent = Image.FromStream(ms);
					}


					string newTitle = TitleHelper.Generate(type, content);

					string hashPart = fileName.Contains("_") ? fileName.Split('_')[1] : fileName;

					var item = new ClipboardItem(type, content, newTitle, imageContent, hashPart);
					item.Id = fileName;
					item.Timestamp = fileInfo.LastWriteTime;
					loadedCache.Add(item);
				}
				catch { try { File.Delete(fileInfo.FullName); } catch { } }
			}
			return loadedCache;
		}


		private void EnforceDiskLimitOnDisk(string historyDir)
		{
			try
			{
				var pinnedIds = TempConfigLoader.Current.PinnedHashes;

				var unpinnedFiles = Directory.GetFiles(historyDir)
					.Select(f => new FileInfo(f))
					.Where(f => !f.Name.Equals("secret.key", StringComparison.OrdinalIgnoreCase))
					.Where(f => !pinnedIds.Contains(Path.GetFileNameWithoutExtension(f.Name)))
					.OrderBy(f => f.LastWriteTime) 
					.ToList();

				if (unpinnedFiles.Count > _maxHistoryCount)
				{
					int toDeleteCount = unpinnedFiles.Count - _maxHistoryCount;

					for (int i = 0; i < toDeleteCount; i++)
					{
						try
						{
							unpinnedFiles[i].Delete();
						}
						catch { /* Skip */ }
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error occured when startup cleaning: {ex.Message}");
			}
		}

	

	

	}
}