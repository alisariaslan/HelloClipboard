using System;
using System.Collections.Generic;
using System.Drawing;

namespace HelloClipboard
{
    public enum ClipboardItemType
    {
        Path,
        Text,
        Image,
    }

    public class ClipboardItem
    {
        public string Id { get; set; }
        public ClipboardItemType ItemType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Image ImageContent { get; set; }
        public DateTime Timestamp { get; set; }
        public string ContentHash { get; set; }
        public bool IsPinned { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public ClipboardItem(ClipboardItemType type, string text, string title, Image image = null, string contentHash = null, bool isPinned = false)
        {
            Id = DateTime.Now.Ticks.ToString() + "_" + (contentHash ?? Guid.NewGuid().ToString().Substring(0, 8));
            ItemType = type;
            Content = text;
            Timestamp = DateTime.Now;
            Title = title;
            ImageContent = image;
            ContentHash = contentHash;
            IsPinned = isPinned;
        }
    }
}
