using System;

namespace HelloClipboard
{
	public class ClipboardItem
	{
		public string Title { get; set; }
		public string Text { get; set; }
		public DateTime Timestamp { get; set; }
		public int Index { get; set; }
		public ClipboardItem(int index,string text, string title)
		{
			Index = index;
			Text = text;
			Timestamp = DateTime.Now;
			Title = title;
		}
	}
}
