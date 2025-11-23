using System;
using System.Drawing;

namespace HelloClipboard
{
	public enum ClipboardItemType
	{
		File,
		Text,
		Image,
	}

	public class ClipboardItem
	{
		public ClipboardItemType ItemType { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public Image ImageContent { get; set; }
		public DateTime Timestamp { get; set; }
		public int Index { get; set; }
		public ClipboardItem(int index,ClipboardItemType type ,string text, string title, Image image = null)
		{
			Index = index;
			ItemType = type;
			Content = text;
			Timestamp = DateTime.Now;
			Title = title;
			ImageContent = image;
		}
	}
}
