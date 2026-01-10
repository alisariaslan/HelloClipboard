using HelloClipboard.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HelloClipboard
{
	public static class FileOpener
	{
		public static void OpenItem(ClipboardItem item)
		{
			if (item == null) return;

			try
			{
				string content = item.Content?.Trim();

				if (item.ItemType == ClipboardItemType.Text && UrlHelper.IsValidUrl(content))
				{
					UrlHelper.OpenUrl(content);
				}
				else if (!string.IsNullOrEmpty(content) && (File.Exists(content) || Directory.Exists(content)))
				{
					OpenPath(content);
				}
				else
				{
					HandleMemoryContent(item);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"General Error: {ex.Message}");
			}
		}

		private static void HandleMemoryContent(ClipboardItem item)
		{
			// Master isimlendirme: HC_[KısaID]_[TemizBaşlık].uzantı
			string fileName = GetUnifiedFileName(item);
			string tempPath = Path.Combine(Path.GetTempPath(), fileName);

			try
			{
				if (item.ItemType == ClipboardItemType.Image && item.ImageContent != null)
				{
					item.ImageContent.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
				}
				else if (item.ItemType == ClipboardItemType.Text && !string.IsNullOrEmpty(item.Content))
				{
					File.WriteAllText(tempPath, item.Content, Encoding.UTF8);
				}

				if (File.Exists(tempPath))
				{
					OpenPath(tempPath);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"File Write Error: {ex.Message}");
			}
		}

		public static void OpenPath(string path)
		{
			if (string.IsNullOrEmpty(path)) return;
			Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
		}

		// BU METOD ViewModel İLE AYNI ÇALIŞMALI
		public static string GetUnifiedFileName(ClipboardItem item)
		{
			string prefix = item.ItemType == ClipboardItemType.Image ? "IMG" : "TXT";
			string safeTitle = Regex.Replace(item.Title ?? "item", @"[^a-zA-Z0-9]", "");
			if (safeTitle.Length > 25) safeTitle = safeTitle.Substring(0, 25);

			string extension = item.ItemType == ClipboardItemType.Image ? ".png" : ".txt";

			// Dosya ismi: HC_TXT_TemizBaslik.txt (Açarken ve Kaydederken aynı olması için)
			return $"HC_{prefix}_{safeTitle}{extension}";
		}
	}
}