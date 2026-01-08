using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HelloClipboard
{
	public class HistoryHelper
	{
		private readonly int _maxHistoryCount;

		public HistoryHelper()
		{
			_maxHistoryCount = SettingsLoader.Current.MaxHistoryCount;
		}

		// HistoryHelper.cs içine eklenecek metod
		public void ClearAllHistoryFiles()
		{
			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) return;

			try
			{
				// Klasördeki her şeyi sil (ancak secret.key gibi anahtar dosyalarını korumak isteyebilirsiniz)
				var files = Directory.GetFiles(historyDir);
				foreach (var file in files)
				{
					// Eğer kripto anahtarını burada saklıyorsanız onu hariç tutun:
					if (Path.GetFileName(file).Equals("secret.key", StringComparison.OrdinalIgnoreCase))
						continue;

					File.Delete(file);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"History dosyaları silinirken hata: {ex.Message}");
			}

		}
		public void SaveItemToHistoryFile(ClipboardItem item)
		{
			if (string.IsNullOrEmpty(item.Id)) return;

			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) Directory.CreateDirectory(historyDir);

			string extension = FileExtensionHelper.GetFileExtension(item.ItemType);
			// ARTIK ContentHash yerine Id kullanıyoruz
			string filePath = Path.Combine(historyDir, item.Id + extension);

			// Aynı Id'ye sahip dosya zaten varsa (örneğin uygulama açıkken tekrar kaydetmeye çalışırsa) yazma
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

		public void DeleteItemFromFile(string id) // Parametre adı hash'den id'ye döndü
		{
			if (string.IsNullOrWhiteSpace(id)) return;
			string historyDir = Constants.HistoryDirectory;
			try
			{
				// Dosyayı Id üzerinden bulup siliyoruz
				var filesToDelete = Directory.GetFiles(historyDir, id + ".*");
				foreach (var file in filesToDelete) File.Delete(file);
			}
			catch (Exception) { /* Debug.WriteLine(ex.Message); */ }
		}

		public List<ClipboardItem> LoadHistoryFromFiles()
		{
			var loadedCache = new List<ClipboardItem>();
			string historyDir = Constants.HistoryDirectory;
			if (!Directory.Exists(historyDir)) return loadedCache;

			var files = Directory.GetFiles(historyDir)
								 .Select(f => new FileInfo(f))
								 .OrderBy(f => f.LastWriteTime)
								 .ToList();

			foreach (var fileInfo in files)
			{
				if (fileInfo.Name.Equals("secret.key")) continue;

				string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
				string extension = Path.GetExtension(fileInfo.Name);
				ClipboardItemType type = FileExtensionHelper.GetItemTypeFromExtension(extension);

				try
				{
					byte[] decryptedBytes = CryptoHelper.Decrypt(File.ReadAllBytes(fileInfo.FullName));
					if (decryptedBytes == null) continue;

					string content = null;
					Image imageContent = null;
					string newTitle = "";

					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
					{
						content = Encoding.UTF8.GetString(decryptedBytes);
						newTitle = type == ClipboardItemType.Text ? GenerateTitle(content) : $"{Path.GetFileName(content)} -> {content}";
					}
					else
					{
						using (var ms = new MemoryStream(decryptedBytes)) imageContent = Image.FromStream(ms);
						newTitle = "[IMAGE]";
					}

					// ÖNEMLİ: Dosyadaki Hash kısmını Id'den ayıklayıp nesneye veriyoruz
					string hashPart = fileName.Contains("_") ? fileName.Split('_')[1] : fileName;

					var item = new ClipboardItem(type, content, newTitle, imageContent, hashPart);
					item.Id = fileName; // Dosya adını Id olarak geri yüklüyoruz
					item.Timestamp = fileInfo.LastWriteTime; // Sıralama için dosya tarihini kullanıyoruz
					loadedCache.Add(item);
				}
				catch { try { File.Delete(fileInfo.FullName); } catch { } }
			}
			return loadedCache;

		}

		private string GenerateTitle(string content)
		{
			if (string.IsNullOrWhiteSpace(content)) return string.Empty;

			// Metni temizle: Satır sonlarını ve tabları boşlukla değiştir, fazla boşlukları sil
			string cleaned = Regex.Replace(content.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' '), @"\s+", " ").Trim();

			// Çok uzunsa kısalt
			return cleaned.Length > 1024 ? cleaned.Substring(0, 1024) + "..." : cleaned;
		}

		//		public List<ClipboardItem> LoadHistoryFromFiles()
		//		{
		//			var loadedCache = new List<ClipboardItem>();
		//			string historyDir = Constants.HistoryDirectory;
		//			if (!Directory.Exists(historyDir))
		//				return loadedCache;

		//			// Filter out files like 'secret.key' or use error handling to prevent loading non-history files
		//			var files = Directory.GetFiles(historyDir)
		//								 .Select(f => new FileInfo(f))
		//								 .OrderBy(f => f.LastWriteTime)
		//								 .ToList();

		//			int count = 0;
		//			int imgCount = 0;
		//			foreach (var fileInfo in files)
		//			{
		//				if (count >= _maxHistoryCount)
		//				{
		//					// Delete excess history files to maintain the limit
		//					try { File.Delete(fileInfo.FullName); } catch { }
		//					continue;
		//				}

		//				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
		//				string extension = Path.GetExtension(fileInfo.Name);

		//				ClipboardItemType type = FileExtensionHelper.GetItemTypeFromExtension(extension);

		//				try
		//				{
		//					string content = null;
		//					Image imageContent = null;
		//					string newTitle = "";

		//					// 1. Read file as binary
		//					byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);

		//					// 2. Decrypt the data
		//					byte[] decryptedBytes = CryptoHelper.Decrypt(fileBytes);

		//					if (decryptedBytes == null) continue; // Skip if decryption fails

		//					if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
		//					{
		//						// Convert byte array back to string
		//						content = Encoding.UTF8.GetString(decryptedBytes);

		//						if (type == ClipboardItemType.Text)
		//						{
		//							// Sanitize text by replacing control characters with spaces
		//							string replacedNewlines = content.Replace('\r', ' ')
		//													 .Replace('\n', ' ')
		//													 .Replace('\t', ' ');
		//							// Collapse multiple whitespaces into a single space
		//							string cleanedWhitespace = Regex.Replace(replacedNewlines, @"\s+", " ");
		//							newTitle = cleanedWhitespace.Trim();

		//							if (newTitle.Length > 1024)
		//							{
		//								newTitle = newTitle.Substring(0, 1024) + "...";
		//							}
		//						}
		//						else if (type == ClipboardItemType.File)
		//						{
		//							newTitle = $"{Path.GetFileName(content)} -> {content}";
		//						}
		//					}
		//					else if (type == ClipboardItemType.Image)
		//					{
		//						// Create image from decrypted byte array
		//						using (var ms = new MemoryStream(decryptedBytes))
		//						{
		//							imageContent = Image.FromStream(ms);
		//						}
		//						newTitle = $"[IMAGE {++imgCount}]";
		//					}

		//					if (content != null || imageContent != null)
		//					{
		//						// Constructor'da index gönderilmiyor
		//						var item = new ClipboardItem(type, content, newTitle, imageContent, fileNameWithoutExtension);

		//						// KRİTİK: RAM'deki nesnenin vaktini, dosyanın gerçek vaktiyle eşleştiriyoruz
		//						item.Timestamp = fileInfo.LastWriteTime;

		//						loadedCache.Add(item);
		//						count++;
		//					}
		//				}
		//				catch (Exception ex)
		//				{
		//#if DEBUG
		//					System.Diagnostics.Debug.WriteLine($"Error loading clipboard history file {fileInfo.Name}: {ex.Message}");
		//#endif
		//					// Delete corrupted files
		//					try { File.Delete(fileInfo.FullName); } catch { }
		//				}
		//			}

		//			return loadedCache;
		//		}


	}
}