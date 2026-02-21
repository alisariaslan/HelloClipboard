using System;
using System.Collections.Generic;

namespace HelloClipboard.Models
{
    public class SnippetItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }

        public SnippetItem()
        {
            Id = Guid.NewGuid().ToString("N")[..8];
            CreatedAt = DateTime.Now;
        }

        public SnippetItem(string name, string content) : this()
        {
            Name = name;
            Content = content;
        }

        public override string ToString() => Name ?? "Untitled Snippet";
    }
}
