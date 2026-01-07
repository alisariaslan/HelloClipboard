using HelloClipboard.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
	public class ClipboardService
	{
		private const int MaxAttempts = 4;
		private const int RetryDelayMs = 25;

		public async Task<ClipboardDataPackage> TryReadClipboardAsync()
		{
			for (int attempt = 0; attempt < MaxAttempts; attempt++)
			{
				try
				{
					IDataObject dataObj = Clipboard.GetDataObject();
					if (dataObj == null) return null;

					// 1. Metin Kontrolü
					if (dataObj.GetDataPresent(DataFormats.UnicodeText, true))
					{
						string text = dataObj.GetData(DataFormats.UnicodeText, true) as string;
						if (!string.IsNullOrEmpty(text))
							return CreatePackage(ClipboardItemType.Text, text);
					}

					// 2. Dosya Kontrolü
					if (dataObj.GetDataPresent(DataFormats.FileDrop))
					{
						string[] files = dataObj.GetData(DataFormats.FileDrop) as string[];
						if (files != null && files.Length > 0)
							return CreatePackage(ClipboardItemType.File, files[0]); // İlk dosyayı alıyoruz
					}

					// 3. Görsel Kontrolü
					if (dataObj.GetDataPresent(DataFormats.Bitmap))
					{
						Image img = Clipboard.GetImage();
						if (img != null)
							return CreatePackage(ClipboardItemType.Image, null, img);
					}
				}
				catch (ExternalException)
				{
					if (attempt < MaxAttempts - 1) await Task.Delay(RetryDelayMs);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Clipboard Read Error: {ex.Message}");
				}
			}
			return null;
		}

		private ClipboardDataPackage CreatePackage(ClipboardItemType type, string content, Image image = null)
		{
			string title = "";
			string hash = "";

			switch (type)
			{
				case ClipboardItemType.Text:
					title = GenerateTextTitle(content);
					hash = HashHelper.CalculateMd5Hash(content);
					break;
				case ClipboardItemType.File:
					title = $"{Path.GetFileName(content)} -> {content}";
					hash = HashHelper.CalculateMd5Hash(content);
					break;
				case ClipboardItemType.Image:
					hash = HashHelper.HashImageBytes(image);
					title = $"[IMAGE]"; // Daha sonra TrayApplicationContext'te sayı eklenebilir
					break;
			}

			return new ClipboardDataPackage
			{
				Type = type,
				Content = content,
				Image = image,
				Title = title,
				Hash = hash
			};
		}

		private string GenerateTextTitle(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return string.Empty;
			string cleaned = Regex.Replace(text.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' '), @"\s+", " ").Trim();
			return cleaned.Length > 1024 ? cleaned.Substring(0, 1024) + "..." : cleaned;
		}
	}

	// Veri transfer nesnesi (DTO)
	public class ClipboardDataPackage
	{
		public ClipboardItemType Type { get; set; }
		public string Content { get; set; }
		public Image Image { get; set; }
		public string Title { get; set; }
		public string Hash { get; set; }
	}
}