using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class SettingsForm : Form
	{
		private Timer _debounceTimer;
		private MainForm _mainForm;
		private Keys _pendingHotkeyModifiers;
		private Keys _pendingHotkeyKey;

		public SettingsForm(MainForm mainForm)
		{
			InitializeComponent();

			_mainForm = mainForm;

			_debounceTimer = new Timer();
			_debounceTimer.Interval = 500;
			_debounceTimer.Tick += DebounceTimer_Tick;

			RemoveSettingEvents();
			checkBox1_suppressClipboardEvents.Checked = SettingsLoader.Current.SuppressClipboardEvents;
			checkBox2_hideToSystemTray.Checked = SettingsLoader.Current.HideToTray;
			checkBox3_checkUpdates.Checked = SettingsLoader.Current.CheckUpdates;
			checkBox1_startWithWindows.Checked = SettingsLoader.Current.StartWithWindows;
			checkBox4_preventClipboardDuplication.Checked = SettingsLoader.Current.PreventClipboardDuplication;
			textBox1_maxHistoryCount.Text = SettingsLoader.Current.MaxHistoryCount.ToString();
			checkBox1_invertClipboardHistoryListing.Checked = SettingsLoader.Current.InvertClipboardHistoryListing;
			checkBox1_clipboardHistory.Checked = SettingsLoader.Current.EnableClipboardHistory;
			checkBox1_alwaysTopMost.Checked = SettingsLoader.Current.AlwaysTopMost;
			checkBox1_showInTaskbar.Checked = SettingsLoader.Current.ShowInTaskbar;
			checkBox1_autoHideWhenUnfocus.Checked = SettingsLoader.Current.AutoHideWhenUnfocus;
			textBox_privacyDuration.Text = SettingsLoader.Current.PrivacyModeDurationMinutes.ToString();
			checkBox_enableHotkey.Checked = SettingsLoader.Current.EnableGlobalHotkey;
			textBox_hotkey.Text = FormatHotkey(SettingsLoader.Current.HotkeyModifiers, SettingsLoader.Current.HotkeyKey);
			textBox_hotkey.Enabled = SettingsLoader.Current.EnableGlobalHotkey;
			_pendingHotkeyKey = SettingsLoader.Current.HotkeyKey;
			_pendingHotkeyModifiers = SettingsLoader.Current.HotkeyModifiers;
			checkBox2_enableTimeStamps.Checked = SettingsLoader.Current.EnableTimeStamps;
			AddSettingEvents();

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

		private void RemoveSettingEvents()
		{
			checkBox1_suppressClipboardEvents.CheckedChanged -= checkBox1_suppressClipboardEvents_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged -= textBox1_maxHistoryCount_TextChanged;
			checkBox3_checkUpdates.CheckedChanged -= checkBox3_checkUpdates_CheckedChanged;
			checkBox1_startWithWindows.CheckedChanged -= checkBox1_startWithWindows_CheckedChanged;
			checkBox2_hideToSystemTray.CheckedChanged -= checkBox2_hideToSystemTray_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged -= checkBox4_preventClipboardDuplication_CheckedChanged;
			checkBox1_invertClipboardHistoryListing.CheckedChanged -= checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.CheckedChanged -= checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.CheckedChanged -= checkBox1_alwaysTopMost_CheckedChanged;
			checkBox1_showInTaskbar.CheckedChanged -= checkBox1_showInTaskbar_CheckedChanged;
			checkBox1_autoHideWhenUnfocus.CheckedChanged -= checkBox1_autoHideWhenUnfocus_CheckedChanged;
			textBox_privacyDuration.KeyPress -= textBox_privacyDuration_KeyPress;
			textBox_privacyDuration.Leave -= textBox_privacyDuration_Leave;
			checkBox_enableHotkey.CheckedChanged -= checkBox_enableHotkey_CheckedChanged;
			textBox_hotkey.KeyDown -= textBox_hotkey_KeyDown;
			checkBox2_enableTimeStamps.CheckedChanged -= checkBox2_enableTimeStamps_CheckedChanged;
		}

		private void AddSettingEvents()
		{
			checkBox1_suppressClipboardEvents.CheckedChanged += checkBox1_suppressClipboardEvents_CheckedChanged;
			textBox1_maxHistoryCount.TextChanged += textBox1_maxHistoryCount_TextChanged;
			checkBox3_checkUpdates.CheckedChanged += checkBox3_checkUpdates_CheckedChanged;
			checkBox1_startWithWindows.CheckedChanged += checkBox1_startWithWindows_CheckedChanged;
			checkBox2_hideToSystemTray.CheckedChanged += checkBox2_hideToSystemTray_CheckedChanged;
			checkBox4_preventClipboardDuplication.CheckedChanged += checkBox4_preventClipboardDuplication_CheckedChanged;
			checkBox1_invertClipboardHistoryListing.CheckedChanged += checkBox1_invertClipboardHistoryListing_CheckedChanged;
			checkBox1_clipboardHistory.CheckedChanged += checkBox1_clipboardHistory_CheckedChanged;
			checkBox1_alwaysTopMost.CheckedChanged += checkBox1_alwaysTopMost_CheckedChanged;
			checkBox1_showInTaskbar.CheckedChanged += checkBox1_showInTaskbar_CheckedChanged;
			checkBox1_autoHideWhenUnfocus.CheckedChanged += checkBox1_autoHideWhenUnfocus_CheckedChanged;
			textBox_privacyDuration.KeyPress += textBox_privacyDuration_KeyPress;
			textBox_privacyDuration.Leave += textBox_privacyDuration_Leave;
			checkBox_enableHotkey.CheckedChanged += checkBox_enableHotkey_CheckedChanged;
			textBox_hotkey.KeyDown += textBox_hotkey_KeyDown;
			checkBox2_enableTimeStamps.CheckedChanged += checkBox2_enableTimeStamps_CheckedChanged;
		}

		private async void button2_Defaults_Click(object sender, EventArgs e)
		{
			if (!await PrivilegesHelper.EnsureAdministrator())
				return;

			var def = new SettingsModel();

			RemoveSettingEvents();
			checkBox1_suppressClipboardEvents.Checked = def.SuppressClipboardEvents;
			textBox1_maxHistoryCount.Text = def.MaxHistoryCount.ToString();
			checkBox3_checkUpdates.Checked = def.CheckUpdates;
			checkBox1_startWithWindows.Checked = def.StartWithWindows;
			checkBox2_hideToSystemTray.Checked = def.HideToTray;
			checkBox4_preventClipboardDuplication.Checked = def.PreventClipboardDuplication;
			checkBox1_invertClipboardHistoryListing.Checked = def.InvertClipboardHistoryListing;
			checkBox1_clipboardHistory.Checked = def.EnableClipboardHistory;
			checkBox1_alwaysTopMost.Checked = def.AlwaysTopMost;
			checkBox1_showInTaskbar.Checked = def.ShowInTaskbar;
			checkBox1_autoHideWhenUnfocus.Checked = def.AutoHideWhenUnfocus;
			textBox_privacyDuration.Text = def.PrivacyModeDurationMinutes.ToString();
			checkBox_enableHotkey.Checked = def.EnableGlobalHotkey;
			textBox_hotkey.Text = FormatHotkey(def.HotkeyModifiers, def.HotkeyKey);
			textBox_hotkey.Enabled = def.EnableGlobalHotkey;
			checkBox2_enableTimeStamps.Checked = def.EnableTimeStamps;
			AddSettingEvents();

			SettingsLoader.Current = def;
			SettingsLoader.Save();
			TrayApplicationContext.Instance?.ReloadGlobalHotkey();
			TrayApplicationContext.Instance?.RefreshPrivacyMenuLabel();

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

		private void textBox_privacyDuration_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsControl(e.KeyChar))
				return;

			var tb = sender as TextBox;
			if (!char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
				return;
			}

			var existingLength = tb?.Text?.Length ?? 0;
			var selectionLength = tb?.SelectionLength ?? 0;
			if (selectionLength == 0 && existingLength >= 2)
				e.Handled = true;
		}

		private void textBox_privacyDuration_Leave(object sender, EventArgs e)
		{
			SavePrivacyDuration();
		}

		private void SavePrivacyDuration()
		{
			var text = textBox_privacyDuration.Text.Trim();
			if (string.IsNullOrEmpty(text))
				return;

			if (!int.TryParse(text, out var minutes))
				return;

			if (minutes < 1)
				minutes = 1;
			if (minutes > 99)
				minutes = 99;

			textBox_privacyDuration.Text = minutes.ToString();

			if (SettingsLoader.Current.PrivacyModeDurationMinutes == minutes)
				return;

			SettingsLoader.Current.PrivacyModeDurationMinutes = minutes;
			SettingsLoader.Save();
			TrayApplicationContext.Instance?.RefreshPrivacyMenuLabel();
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

		private void checkBox1_showInTaskbar_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.ShowInTaskbar = checkBox1_showInTaskbar.Checked;
			SettingsLoader.Save();

			_mainForm.UpdateTaskbarVisibility(checkBox1_showInTaskbar.Checked);

			// Ayar değiştiğinde bu form açık kalmasın; yeni duruma göre ana pencere davranışı uygular
			this.Close();
		}

		private void checkBox1_autoHideWhenUnfocus_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.AutoHideWhenUnfocus = checkBox1_autoHideWhenUnfocus.Checked;
			SettingsLoader.Save();
		}

		private void checkBox_enableHotkey_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.EnableGlobalHotkey = checkBox_enableHotkey.Checked;
			textBox_hotkey.Enabled = checkBox_enableHotkey.Checked;
			SettingsLoader.Save();
			TrayApplicationContext.Instance?.ReloadGlobalHotkey();
		}

		private void textBox_hotkey_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			var key = e.KeyCode;
			var mods = NormalizeModifiers(e.Modifiers);

			// Sadece mod tuşuna basılmışsa bekle (uyarı verme)
			if (IsModifierKey(key))
				return;

			if (mods == Keys.None)
			{
				MessageBox.Show("En az bir mod tuşu (Ctrl, Alt, Shift veya Win) kullanmalısınız.", "Geçersiz kısayol", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_pendingHotkeyKey = key;
			_pendingHotkeyModifiers = mods;

			SettingsLoader.Current.HotkeyKey = _pendingHotkeyKey;
			SettingsLoader.Current.HotkeyModifiers = _pendingHotkeyModifiers;
			SettingsLoader.Current.EnableGlobalHotkey = true;

			checkBox_enableHotkey.Checked = true;
			textBox_hotkey.Text = FormatHotkey(_pendingHotkeyModifiers, _pendingHotkeyKey);
			SettingsLoader.Save();
			TrayApplicationContext.Instance?.ReloadGlobalHotkey();
		}

		private string FormatHotkey(Keys modifiers, Keys key)
		{
			var parts = new System.Collections.Generic.List<string>();
			if (modifiers.HasFlag(Keys.Control)) parts.Add("Ctrl");
			if (modifiers.HasFlag(Keys.Shift)) parts.Add("Shift");
			if (modifiers.HasFlag(Keys.Alt)) parts.Add("Alt");
			if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin)) parts.Add("Win");

			if (key != Keys.None)
			{
				parts.Add(key.ToString());
			}
			return string.Join(" + ", parts);
		}

		private Keys NormalizeModifiers(Keys modifiers)
		{
			Keys result = Keys.None;
			if (modifiers.HasFlag(Keys.Control)) result |= Keys.Control;
			if (modifiers.HasFlag(Keys.Shift)) result |= Keys.Shift;
			if (modifiers.HasFlag(Keys.Alt)) result |= Keys.Alt;
			if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin)) result |= Keys.LWin;
			return result;
		}

		private bool IsModifierKey(Keys key)
		{
			return key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Menu || key == Keys.LWin || key == Keys.RWin;
		}

		private void checkBox1_suppressClipboardEvents_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.SuppressClipboardEvents = checkBox1_suppressClipboardEvents.Checked;
			SettingsLoader.Save();
		}

		private void checkBox2_enableTimeStamps_CheckedChanged(object sender, EventArgs e)
		{
			SettingsLoader.Current.EnableTimeStamps = checkBox2_enableTimeStamps.Checked;
			SettingsLoader.Save();
			// Liste görünümünü anında güncellemek için
			_mainForm.RefreshCacheView();
		}
	}
}
