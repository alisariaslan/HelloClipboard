using System.IO;
using System.Text.RegularExpressions;

namespace HelloClipboard.Utils
{
	public static class TitleHelper
	{
		private static int _imgCount = 0;
		public static string Generate(ClipboardItemType type, string content)
		{
			switch (type)
			{
				case ClipboardItemType.Text:
					if (string.IsNullOrWhiteSpace(content)) return "Empty Text";
					string cleaned = Regex.Replace(content.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' '), @"\s+", " ").Trim();
					return cleaned.Length > 1024 ? cleaned.Substring(0, 1024) + "..." : cleaned;

				case ClipboardItemType.File:
					return $"{Path.GetFileName(content)} -> {content}";

				case ClipboardItemType.Image:
					_imgCount++;
					return $"[IMAGE {_imgCount}]";

				default:
					return "Unknown Content";
			}
		}
	}
}