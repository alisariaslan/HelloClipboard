using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class SettingsForm : Form
	{
		private Timer _debounceTimer;
		private MainForm _mainForm;

		public SettingsForm(MainForm mainForm)
		{
			InitializeComponent();

			_mainForm = mainForm;

			_debounceTimer = new Timer();
			_debounceTimer.Interval = 500;
			_debounceTimer.Tick += DebounceTimer_Tick;

			checkBox2_hideToSystemTray.CheckedChanged -= checkBox2_hideToSystemTray_CheckedChanged;
			checkBox3_checkUpdates.CheckedChanged -= checkBox3_checkUpdates_CheckedChanged;
			checkBox1_startWithWindows.CheckedChanged -= checkBox1_startWithWindows_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged -= checkBox4_preventClipboardDuplication_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged -= textBox1_maxHistoryCount_TextChanged;
			checkBox1_invertClipboardHistoryListing.TextChanged -= checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.TextChanged -= checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.TextChanged -= checkBox1_alwaysTopMost_CheckedChanged;

			checkBox2_hideToSystemTray.Checked = SettingsLoader.Current.HideToTray;
			checkBox3_checkUpdates.Checked = SettingsLoader.Current.CheckUpdates;
			checkBox1_startWithWindows.Checked = SettingsLoader.Current.StartWithWindows;
			checkBox4_preventClipboardDuplication.Checked = SettingsLoader.Current.PreventClipboardDuplication;
			textBox1_maxHistoryCount.Text = SettingsLoader.Current.MaxHistoryCount.ToString();
			checkBox1_invertClipboardHistoryListing.Checked = SettingsLoader.Current.InvertClipboardHistoryListing;
			checkBox1_clipboardHistory.Checked = SettingsLoader.Current.EnableClipboardHistory;
			checkBox1_alwaysTopMost.Checked = SettingsLoader.Current.AlwaysTopMost;

			checkBox2_hideToSystemTray.CheckedChanged += checkBox2_hideToSystemTray_CheckedChanged;
			checkBox3_checkUpdates.CheckedChanged += checkBox3_checkUpdates_CheckedChanged;
			checkBox1_startWithWindows.CheckedChanged += checkBox1_startWithWindows_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged += checkBox4_preventClipboardDuplication_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged += textBox1_maxHistoryCount_TextChanged;
			checkBox1_invertClipboardHistoryListing.TextChanged += checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.TextChanged += checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.TextChanged += checkBox1_alwaysTopMost_CheckedChanged;
		}


		private void textBox1_maxHistoryCount_TextChanged(object sender, EventArgs e)
		{
			_debounceTimer.Stop();
			_debounceTimer.Start();
		}

		private void textBox1_maxHistoryCount_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsControl(e.KeyChar))
				return;

			if (!char.IsDigit(e.KeyChar))
				e.Handled = true;
		}

		private void DebounceTimer_Tick(object sender, EventArgs e)
		{
			_debounceTimer.Stop();
			SaveMaxHistory();
		}

		private void textBox1_maxHistoryCount_Leave(object sender, EventArgs e)
		{
			SaveMaxHistory();
		}

		private void SaveMaxHistory()
		{
			string text = textBox1_maxHistoryCount.Text.Trim();

			if (string.IsNullOrEmpty(text))
				return;

			if (!int.TryParse(text, out int value))
				return;

			if (value < 10)
				value = 10;

			if (value > 10000)
				value = 10000;

			if (SettingsLoader.Current.MaxHistoryCount == value)
				return;

			SettingsLoader.Current.MaxHistoryCount = value;
			SettingsLoader.Save();
		}

		private async void button2_Defaults_Click(object sender, EventArgs e)
		{
			if (!await PrivilegesHelper.EnsureAdministrator())
				return;

			var def = new SettingsModel(); 

			checkBox1_startWithWindows.CheckedChanged -= checkBox1_startWithWindows_CheckedChanged;
			checkBox2_hideToSystemTray.CheckedChanged -= checkBox2_hideToSystemTray_CheckedChanged;
			checkBox3_checkUpdates.CheckedChanged -= checkBox3_checkUpdates_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged -= checkBox4_preventClipboardDuplication_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged -= textBox1_maxHistoryCount_TextChanged;
			checkBox1_invertClipboardHistoryListing.TextChanged -= checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.TextChanged -= checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.TextChanged -= checkBox1_alwaysTopMost_CheckedChanged;

			checkBox1_startWithWindows.Checked = def.StartWithWindows;
			checkBox2_hideToSystemTray.Checked = def.HideToTray;
			checkBox3_checkUpdates.Checked = def.CheckUpdates;
			checkBox4_preventClipboardDuplication.Checked = def.PreventClipboardDuplication;
			textBox1_maxHistoryCount.Text = def.MaxHistoryCount.ToString();
			checkBox1_invertClipboardHistoryListing.Checked = def.InvertClipboardHistoryListing;
			checkBox1_clipboardHistory.Checked = def.EnableClipboardHistory;
			checkBox1_alwaysTopMost.Checked = def.AlwaysTopMost;

			checkBox1_startWithWindows.CheckedChanged += checkBox1_startWithWindows_CheckedChanged;
			checkBox2_hideToSystemTray.CheckedChanged += checkBox2_hideToSystemTray_CheckedChanged;
			checkBox3_checkUpdates.CheckedChanged += checkBox3_checkUpdates_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged += checkBox4_preventClipboardDuplication_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged += textBox1_maxHistoryCount_TextChanged;
			checkBox1_invertClipboardHistoryListing.TextChanged += checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.TextChanged += checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.TextChanged += checkBox1_alwaysTopMost_CheckedChanged;

			SettingsLoader.Current = def;
			SettingsLoader.Save();

			try
			{
				string appName = Constants.AppName;
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
						   @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
				{
					if (def.StartWithWindows)
						key.SetValue(appName, $"\"{Application.ExecutablePath}\"");
					else if (key.GetValue(appName) != null)
						key.DeleteValue(appName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.Message,
					"Failed to update application startup (Registry)",
					MessageBoxButtons.OK);
			}

			MessageBox.Show("All settings have been reset to default.", "Defaults", MessageBoxButtons.OK);

		}


		private async void checkBox1_startWithWindows_CheckedChanged(object sender, EventArgs e)
		{
			if (!await PrivilegesHelper.EnsureAdministrator())
				return;
			try
			{
				string appName = Constants.AppName;
				string exePath = $"\"{Application.ExecutablePath}\"";
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
						   @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
				{
					if (checkBox1_startWithWindows.Checked)
					{
						key.SetValue(appName, exePath);
					}
					else
					{
						if (key.GetValue(appName) != null)
							key.DeleteValue(appName);
					}
				}
				SettingsLoader.Current.StartWithWindows = checkBox1_startWithWindows.Checked;
				SettingsLoader.Save();
			}
			catch (Exception ex)
			{
				MessageBox.Show(
				ex.Message,
					"Failed to update application startup (Registry)",
					MessageBoxButtons.OK);
			}
		}
		private void checkBox2_hideToSystemTray_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.HideToTray = checkBox2_hideToSystemTray.Checked;
			SettingsLoader.Save();
		}

		private void checkBox3_checkUpdates_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.CheckUpdates = checkBox3_checkUpdates.Checked;
			SettingsLoader.Save();
		}
		private void checkBox4_preventClipboardDuplication_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.PreventClipboardDuplication = checkBox4_preventClipboardDuplication.Checked;
			SettingsLoader.Save();
		}

		private void checkBox1_invertClipboardHistoryListing_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.InvertClipboardHistoryListing = checkBox1_invertClipboardHistoryListing.Checked;
			SettingsLoader.Save();
			_mainForm.RefreshCacheView();
		}

		private void checkBox1_clipboardHistory_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.EnableClipboardHistory = checkBox1_clipboardHistory.Checked;
			SettingsLoader.Save();
		}

		private void checkBox1_alwaysTopMost_CheckedChanged(object sender, EventArgs e)
		{
			_mainForm.TopMost = checkBox1_alwaysTopMost.Checked;
			_mainForm.CheckAndUpdateTopMostImage();
			SettingsLoader.Current.AlwaysTopMost = checkBox1_alwaysTopMost.Checked;
			SettingsLoader.Save();
		}
	}
}
