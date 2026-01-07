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

		/// <summary>
		/// Attempts to read the current clipboard content with a retry mechanism.
		/// This handles cases where the clipboard might be locked by another process.
		/// </summary>
		public async Task<ClipboardDataPackage> TryReadClipboardAsync()
		{
			for (int attempt = 0; attempt < MaxAttempts; attempt++)
			{
				try
				{
					IDataObject dataObj = Clipboard.GetDataObject();
					if (dataObj == null) return null;

					// 1. Check for Text content
					if (dataObj.GetDataPresent(DataFormats.UnicodeText, true))
					{
						string text = dataObj.GetData(DataFormats.UnicodeText, true) as string;
						if (!string.IsNullOrEmpty(text))
							return CreatePackage(ClipboardItemType.Text, text);
					}

					// 2. Check for File/Folder drops
					if (dataObj.GetDataPresent(DataFormats.FileDrop))
					{
						string[] files = dataObj.GetData(DataFormats.FileDrop) as string[];
						if (files != null && files.Length > 0)
							// Capture the first file path as the representative content
							return CreatePackage(ClipboardItemType.File, files[0]);
					}

					// 3. Check for Image/Bitmap content
					if (dataObj.GetDataPresent(DataFormats.Bitmap))
					{
						Image img = Clipboard.GetImage();
						if (img != null)
							return CreatePackage(ClipboardItemType.Image, null, img);
					}
				}
				catch (ExternalException)
				{
					// Clipboard is likely being accessed by another application; retry after delay
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
					// Format title to show filename and full path
					title = $"{Path.GetFileName(content)} -> {content}";
					hash = HashHelper.CalculateMd5Hash(content);
					break;
				case ClipboardItemType.Image:
					hash = HashHelper.HashImageBytes(image);
					// Generic title, index/count can be appended in TrayApplicationContext if needed
					title = $"[IMAGE]";
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

			// Sanitize text: replace newlines/tabs with spaces and collapse multiple spaces for a clean UI title
			string cleaned = Regex.Replace(text.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' '), @"\s+", " ").Trim();
			return cleaned.Length > 1024 ? cleaned.Substring(0, 1024) + "..." : cleaned;
		}
	}

	/// <summary>
	/// Data Transfer Object (DTO) for passing captured clipboard data between services.
	/// </summary>
	public class ClipboardDataPackage
	{
		public ClipboardItemType Type { get; set; }
		public string Content { get; set; }
		public Image Image { get; set; }
		public string Title { get; set; }
		public string Hash { get; set; }
	}
}