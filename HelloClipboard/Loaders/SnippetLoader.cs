using HelloClipboard.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace HelloClipboard
{
    public static class SnippetLoader
    {
        public static List<SnippetItem> Items { get; set; } = new List<SnippetItem>();

        public static void Load()
        {
            string path = AppConstants.SnippetsPath;

            if (!File.Exists(path))
            {
                Items = new List<SnippetItem>();
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                Items = JsonSerializer.Deserialize<List<SnippetItem>>(json) ?? new List<SnippetItem>();
            }
            catch
            {
                Items = new List<SnippetItem>();
            }
        }

        public static void Save()
        {
            try
            {
                string path = AppConstants.SnippetsPath;
                string folder = Path.GetDirectoryName(path);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string json = JsonSerializer.Serialize(Items, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save snippets.\nError: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Add(SnippetItem item)
        {
            Items.Add(item);
            Save();
        }

        public static void Remove(SnippetItem item)
        {
            Items.Remove(item);
            Save();
        }

        public static void Update(SnippetItem item, string name, string content)
        {
            item.Name = name;
            item.Content = content;
            Save();
        }
    }
}
