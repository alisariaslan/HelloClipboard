using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text; // Encoding için gerekli
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

			// Dosya uzantısını belirle (yine de şifreli olacak ama dosya tipini anlamak için tutuyoruz)
			string extension = FileExtensionHelper.GetFileExtension(item.ItemType);
			string filePath = Path.Combine(historyDir, item.ContentHash + extension);

			if (File.Exists(filePath))
				return;

			try
			{
				byte[] rawData = null;

				if (item.ItemType == ClipboardItemType.Text || item.ItemType == ClipboardItemType.File)
				{
					// Metni UTF8 byte dizisine çevir
					rawData = Encoding.UTF8.GetBytes(item.Content);
				}
				else if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
				{
					// Resmi MemoryStream ile byte dizisine çevir
					using (var ms = new MemoryStream())
					{
						item.ImageContent.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						rawData = ms.ToArray();
					}
				}

				// Veriyi şifrele ve diske yaz
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
			// Bu metod değişmedi, dosya silme işlemi aynıdır.
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

			// secret.key dosyasını listeye almamak için filtreleme yapalım veya hata yönetimini kullanalım
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

					// 1. Dosyayı binary olarak oku
					byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);

					// 2. Şifreyi çöz
					byte[] decryptedBytes = CryptoHelper.Decrypt(fileBytes);

					if (decryptedBytes == null) continue; // Şifre çözülemediyse atla

					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
					{
						// Byte dizisini tekrar stringe çevir
						content = Encoding.UTF8.GetString(decryptedBytes);

						if (type == ClipboardItemType.Text)
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
						else if (type == ClipboardItemType.File)
						{
							newTitle = $"{Path.GetFileName(content)} -> {content}";
						}
					}
					else if (type == ClipboardItemType.Image)
					{
						// Decrypted byte array'den resim oluştur
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
					// Bozuk dosyaları sil
					try { File.Delete(fileInfo.FullName); } catch { }
				}
			}

			return loadedCache;
		}
	}
}