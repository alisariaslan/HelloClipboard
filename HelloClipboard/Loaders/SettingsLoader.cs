using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class SettingsModel
	{
		public bool HideToTray { get; set; } = true;
		public bool CheckUpdates { get; set; } = true;
		public bool StartWithWindows { get; set; } = false;
		public bool PreventClipboardDuplication { get; set; } = false;
		public bool EnableBetterHistoryVisualization { get; set; } = true;
		public int MaxHistoryCount { get; set; } = 1000;
	}

	public static class SettingsLoader
	{
		public static SettingsModel Current { get; set; }

		public static void LoadSettings()
		{
			string path = Constants.AppSettingsPath;

			if (!File.Exists(path))
			{
				Current = new SettingsModel();
				Save();
				return;
			}
			try
			{
				string json = File.ReadAllText(path);
				Current = JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
			}
			catch
			{
				Current = new SettingsModel();
				Save();
			}
		}

		public static void Save()
		{
			try
			{
				string path = Constants.AppSettingsPath;
				string folder = Path.GetDirectoryName(path);

				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

				string json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(path, json);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to save settings.\nError: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
