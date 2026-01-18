using HelloClipboard.Models;
using HelloClipboard.Utils;
using Microsoft.Win32;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Enum.Poison;
using ReaLTaiizor.Forms;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class SettingsForm : PoisonForm
    {
        private Timer _debounceTimer;
        private MainForm _mainForm;
        private Keys _pendingHotkeyModifiers;
        private Keys _pendingHotkeyKey;

        public SettingsForm(MainForm mainForm)
        {

            InitializeComponent();

            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            UpdateThemeDropdownText();

            _mainForm = mainForm;

            _debounceTimer = new Timer();
            _debounceTimer.Interval = 500;
            _debounceTimer.Tick += DebounceTimer_Tick;

            RemoveSettingEvents();

            //GENERAL
            poisonToggle1_checkUpdates.Checked = SettingsLoader.Current.CheckUpdates;
            poisonToggle1_startWithWindows.Checked = SettingsLoader.Current.StartWithWindows;
            //APPEARANCE
            poisonToggle1_invertClipboard.Checked = SettingsLoader.Current.InvertClipboardHistoryListing;
            poisonToggle1_alwaysTopMost.Checked = SettingsLoader.Current.AlwaysTopMost;
            poisonToggle1_showInTaskbar.Checked = SettingsLoader.Current.ShowInTaskbar;
            poisonToggle1_showTimeStamps.Checked = SettingsLoader.Current.EnableTimeStamps;
            //HISTORY
            poisonToggle1_enableHistory.Checked = SettingsLoader.Current.EnableClipboardHistory;
            poisonTextBox1_maxHistoryCount.Text = SettingsLoader.Current.MaxHistoryCount.ToString();
            //BEHAVIOUR
            poisonToggle1_hideToSystemTray.Checked = SettingsLoader.Current.HideToTray;
            poisonToggle1_autoHide.Checked = SettingsLoader.Current.AutoHideWhenUnfocus;
            poisonToggle1_preventDuplication.Checked = SettingsLoader.Current.PreventClipboardDuplication;
            poisonToggle1_suppressClipboardEvents.Checked = SettingsLoader.Current.SuppressClipboardEvents;
            poisonToggle1_focusDetailWindow.Checked = SettingsLoader.Current.FocusDetailWindow;
            poisonTextBox1_privateModeDuration.Text = SettingsLoader.Current.PrivacyModeDurationMinutes.ToString();
            poisonToggle1_quickPaste.Checked = SettingsLoader.Current.QuickPasteOnEnter;
            //HOTKEY
            poisonToggle1_globalHotkeys.Checked = SettingsLoader.Current.EnableGlobalHotkey;
            poisonTextBox1_showWindowHotkey.Text = FormatHotkey(SettingsLoader.Current.HotkeyModifiers, SettingsLoader.Current.HotkeyKey);
            poisonTextBox1_showWindowHotkey.Enabled = SettingsLoader.Current.EnableGlobalHotkey;
            _pendingHotkeyKey = SettingsLoader.Current.HotkeyKey;
            _pendingHotkeyModifiers = SettingsLoader.Current.HotkeyModifiers;

            AddSettingEvents();

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
            string text = poisonTextBox1_maxHistoryCount.Text.Trim();

            if (string.IsNullOrEmpty(text))
                return;

            if (!int.TryParse(text, out int value))
                return;

            if (value < 10)
                value = 10;

            if (value > 10000)
                value = 10000;

            poisonTextBox1_maxHistoryCount.Text = value.ToString();

            if (SettingsLoader.Current.MaxHistoryCount == value)
                return;

            SettingsLoader.Current.MaxHistoryCount = value;
            SettingsLoader.Save();
        }

        private void RemoveSettingEvents()
        {
            //GENERAL
            poisonToggle1_checkUpdates.CheckedChanged -= poisonToggle1_checkUpdates_CheckedChanged;
            poisonToggle1_startWithWindows.CheckedChanged -= poisonToggle1_startWithWindows_CheckedChanged;
            //APPEARANCE
            poisonToggle1_invertClipboard.CheckedChanged -= poisonToggle1_invertClipboard_CheckedChanged;
            poisonToggle1_alwaysTopMost.CheckedChanged -= poisonToggle1_alwaysTopMost_CheckedChanged;
            poisonToggle1_showInTaskbar.CheckedChanged -= poisonToggle1_showInTaskbar_CheckedChanged;
            poisonToggle1_showTimeStamps.CheckedChanged -= poisonToggle1_showTimeStamps_CheckedChanged;
            //HISTORY
            poisonToggle1_enableHistory.CheckedChanged -= poisonToggle1_enableHistory_CheckedChanged;
            poisonTextBox1_maxHistoryCount.TextChanged -= poisonTextBox1_maxHistoryCount_TextChanged;
            //BEHAVIOUR
            poisonToggle1_hideToSystemTray.CheckedChanged -= poisonToggle1_hideToSystemTray_CheckedChanged;
            poisonToggle1_autoHide.CheckedChanged -= poisonToggle1_autoHide_CheckedChanged;
            poisonToggle1_preventDuplication.CheckedChanged -= poisonToggle1_preventDuplication_CheckedChanged;
            poisonToggle1_suppressClipboardEvents.CheckedChanged -= poisonToggle1_suppressClipboardEvents_CheckedChanged;
            poisonToggle1_focusDetailWindow.CheckedChanged -= poisonToggle1_focusDetailWindow_CheckedChanged;
            poisonTextBox1_privateModeDuration.KeyPress -= poisonTextBox1_privateModeDuration_KeyPress;
            poisonTextBox1_privateModeDuration.Leave -= poisonTextBox1_privateModeDuration_Leave;
            poisonToggle1_quickPaste.CheckedChanged -= poisonToggle1_quickPaste_CheckedChanged;
            //HOTKEY
            poisonToggle1_globalHotkeys.CheckedChanged -= poisonToggle1_globalHotkeys_CheckedChanged;
            poisonTextBox1_showWindowHotkey.KeyDown -= poisonTextBox1_showWindowHotkey_KeyDown;
        }

        private void AddSettingEvents()
        {
            //GENERAL
            poisonToggle1_checkUpdates.CheckedChanged += poisonToggle1_checkUpdates_CheckedChanged;
            poisonToggle1_startWithWindows.CheckedChanged += poisonToggle1_startWithWindows_CheckedChanged;
            //APPEARANCE
            poisonToggle1_invertClipboard.CheckedChanged += poisonToggle1_invertClipboard_CheckedChanged;
            poisonToggle1_alwaysTopMost.CheckedChanged += poisonToggle1_alwaysTopMost_CheckedChanged;
            poisonToggle1_showInTaskbar.CheckedChanged += poisonToggle1_showInTaskbar_CheckedChanged;
            poisonToggle1_showTimeStamps.CheckedChanged += poisonToggle1_showTimeStamps_CheckedChanged;
            //HISTORY
            poisonToggle1_enableHistory.CheckedChanged += poisonToggle1_enableHistory_CheckedChanged;
            poisonTextBox1_maxHistoryCount.TextChanged += poisonTextBox1_maxHistoryCount_TextChanged;
            //BEHAVIOUR
            poisonToggle1_hideToSystemTray.CheckedChanged += poisonToggle1_hideToSystemTray_CheckedChanged;
            poisonToggle1_autoHide.CheckedChanged += poisonToggle1_autoHide_CheckedChanged;
            poisonToggle1_preventDuplication.CheckedChanged += poisonToggle1_preventDuplication_CheckedChanged;
            poisonToggle1_suppressClipboardEvents.CheckedChanged += poisonToggle1_suppressClipboardEvents_CheckedChanged;
            poisonToggle1_focusDetailWindow.CheckedChanged += poisonToggle1_focusDetailWindow_CheckedChanged;
            poisonTextBox1_privateModeDuration.KeyPress += poisonTextBox1_privateModeDuration_KeyPress;
            poisonTextBox1_privateModeDuration.Leave += poisonTextBox1_privateModeDuration_Leave;
            poisonToggle1_quickPaste.CheckedChanged += poisonToggle1_quickPaste_CheckedChanged;
            //HOTKEY
            poisonToggle1_globalHotkeys.CheckedChanged += poisonToggle1_globalHotkeys_CheckedChanged;
            poisonTextBox1_showWindowHotkey.KeyDown += poisonTextBox1_showWindowHotkey_KeyDown;
        }

        private async void poisonButton1_resetDefaults_Click(object sender, EventArgs e)
        {
            //if (!await PrivilegesHelper.EnsureAdministrator())
            //    return;

            var result = MessageBox.Show(
      "Are you sure you want to reset all settings to default?",
      "Confirm Reset",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Warning
  );

            if (result != DialogResult.Yes)
                return;

            var def = new SettingsModel();

            RemoveSettingEvents();

            //GENERAL
            poisonToggle1_checkUpdates.Checked = def.CheckUpdates;
            poisonToggle1_startWithWindows.Checked = def.StartWithWindows;
            //APPEARANCE
            poisonToggle1_invertClipboard.Checked = def.InvertClipboardHistoryListing;
            poisonToggle1_alwaysTopMost.Checked = def.AlwaysTopMost;
            poisonToggle1_showInTaskbar.Checked = def.ShowInTaskbar;
            poisonToggle1_showTimeStamps.Checked = def.EnableTimeStamps;
            //HISTORY
            poisonToggle1_enableHistory.Checked = def.EnableClipboardHistory;
            poisonTextBox1_maxHistoryCount.Text = def.MaxHistoryCount.ToString();
            //BEHAVIOUR
            poisonToggle1_hideToSystemTray.Checked = def.HideToTray;
            poisonToggle1_autoHide.Checked = def.AutoHideWhenUnfocus;
            poisonToggle1_preventDuplication.Checked = def.PreventClipboardDuplication;
            poisonToggle1_suppressClipboardEvents.Checked = def.SuppressClipboardEvents;
            poisonTextBox1_privateModeDuration.Text = def.PrivacyModeDurationMinutes.ToString();
            poisonToggle1_focusDetailWindow.Checked = def.FocusDetailWindow;
            poisonToggle1_quickPaste.Checked = def.QuickPasteOnEnter;
            //HOTKEY
            poisonToggle1_globalHotkeys.Checked = def.EnableGlobalHotkey;
            poisonTextBox1_showWindowHotkey.Text = FormatHotkey(def.HotkeyModifiers, def.HotkeyKey);
            poisonTextBox1_showWindowHotkey.Enabled = def.EnableGlobalHotkey;

            AddSettingEvents();
            SettingsLoader.Current = def;
            SettingsLoader.Save();
            TrayApplicationContext.Instance?.ReloadGlobalHotkey();
            TrayApplicationContext.Instance?.RefreshPrivacyMenuLabel();
            try
            {
                string appName = AppConstants.AppName;
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

        #region GENERAL
        private void poisonToggle1_checkUpdates_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.CheckUpdates = poisonToggle1_checkUpdates.Checked;
            SettingsLoader.Save();
        }

        private async void poisonToggle1_startWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (!await PrivilegesHelper.EnsureAdministrator())
                return;
            try
            {
                string appName = AppConstants.AppName;
                string exePath = $"\"{Application.ExecutablePath}\"";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                           @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
                {
                    if (poisonToggle1_startWithWindows.Checked)
                    {
                        key.SetValue(appName, exePath);
                    }
                    else
                    {
                        if (key.GetValue(appName) != null)
                            key.DeleteValue(appName);
                    }
                }
                SettingsLoader.Current.StartWithWindows = poisonToggle1_startWithWindows.Checked;
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
        #endregion

        #region APPARANCE
        private void poisonToggle1_invertClipboard_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.InvertClipboardHistoryListing = poisonToggle1_invertClipboard.Checked;
            SettingsLoader.Save();
            _mainForm.RefreshList();
        }
        private void poisonToggle1_alwaysTopMost_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.AlwaysTopMost = poisonToggle1_alwaysTopMost.Checked;
            SettingsLoader.Save();
            _mainForm.ApplyFormBehaviorSettings(true);
        }
        private void poisonToggle1_showInTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.ShowInTaskbar = poisonToggle1_showInTaskbar.Checked;
            SettingsLoader.Save();
            _mainForm.ApplyFormBehaviorSettings(true);
            this.Close();
        }
        private void poisonToggle1_showTimeStamps_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.EnableTimeStamps = poisonToggle1_showTimeStamps.Checked;
            SettingsLoader.Save();
            _mainForm.RefreshList();
        }
        #endregion

        #region HISTORY
        private void poisonToggle1_enableHistory_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.EnableClipboardHistory = poisonToggle1_enableHistory.Checked;
            SettingsLoader.Save();
        }
        private void poisonTextBox1_maxHistoryCount_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
        #endregion

        #region BEHAVIOURS
        private void poisonToggle1_hideToSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.HideToTray = poisonToggle1_hideToSystemTray.Checked;
            SettingsLoader.Save();
            _mainForm.ApplyFormBehaviorSettings(true);
        }
        private void poisonToggle1_autoHide_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.AutoHideWhenUnfocus = poisonToggle1_autoHide.Checked;
            SettingsLoader.Save();
            _mainForm.ApplyFormBehaviorSettings(true);
        }
        private void poisonToggle1_preventDuplication_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.PreventClipboardDuplication = poisonToggle1_preventDuplication.Checked;
            SettingsLoader.Save();
        }
        private void poisonToggle1_suppressClipboardEvents_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.SuppressClipboardEvents = poisonToggle1_suppressClipboardEvents.Checked;
            SettingsLoader.Save();
        }
        private void poisonToggle1_focusDetailWindow_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.FocusDetailWindow = poisonToggle1_focusDetailWindow.Checked;
            SettingsLoader.Save();
            _mainForm.UpdateDetailWindowKeyPreview(poisonToggle1_focusDetailWindow.Checked);
            _mainForm.ApplyFormBehaviorSettings(true);
        }
        private void poisonTextBox1_privateModeDuration_KeyPress(object sender, KeyPressEventArgs e)
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
        private void poisonTextBox1_privateModeDuration_Leave(object sender, EventArgs e)
        {
            SavePrivacyDuration();
        }
        private void SavePrivacyDuration()
        {
            var text = poisonTextBox1_privateModeDuration.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;
            if (!int.TryParse(text, out var minutes))
                return;
            if (minutes < 1)
                minutes = 1;
            if (minutes > 60)
                minutes = 60;
            poisonTextBox1_privateModeDuration.Text = minutes.ToString();
            if (SettingsLoader.Current.PrivacyModeDurationMinutes == minutes)
                return;
            SettingsLoader.Current.PrivacyModeDurationMinutes = minutes;
            SettingsLoader.Save();
            TrayApplicationContext.Instance?.RefreshPrivacyMenuLabel();

        }
        #endregion

        #region HOTKEYS
        private void poisonToggle1_globalHotkeys_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.EnableGlobalHotkey = poisonToggle1_globalHotkeys.Checked;
            poisonTextBox1_showWindowHotkey.Enabled = poisonToggle1_globalHotkeys.Checked;
            SettingsLoader.Save();
            TrayApplicationContext.Instance?.ReloadGlobalHotkey();
        }
        private void poisonTextBox1_showWindowHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            var key = e.KeyCode;
            var mods = NormalizeModifiers(e.Modifiers);
            if (IsModifierKey(key))
                return;
            if (mods == Keys.None)
            {
                MessageBox.Show("You must use at least one modifier key (Ctrl, Alt, Shift, or Win).", "Invalid Shortcut", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _pendingHotkeyKey = key;
            _pendingHotkeyModifiers = mods;
            SettingsLoader.Current.HotkeyKey = _pendingHotkeyKey;
            SettingsLoader.Current.HotkeyModifiers = _pendingHotkeyModifiers;
            SettingsLoader.Current.EnableGlobalHotkey = true;
            poisonToggle1_globalHotkeys.Checked = true;
            poisonTextBox1_showWindowHotkey.Text = FormatHotkey(_pendingHotkeyModifiers, _pendingHotkeyKey);
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
            return string.Join("+", parts);
        }
        private Keys NormalizeModifiers(Keys modifiers)
        {
            Keys result = Keys.None;
            if (modifiers.HasFlag(Keys.Control)) result |= Keys.Control;
            if (modifiers.HasFlag(Keys.Shift)) result |= Keys.Shift;
            if (modifiers.HasFlag(Keys.Alt)) result |= Keys.Alt;
            if (modifiers.HasFlag(Keys.LWin)) result |= Keys.LWin;
            if (modifiers.HasFlag(Keys.RWin)) result |= Keys.RWin;
            return result;
        }
        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Menu || key == Keys.LWin || key == Keys.RWin;
        }
        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _debounceTimer?.Stop();
            _debounceTimer?.Dispose();
            base.OnFormClosed(e);
        }

        #region MANUAL RESIZE (BORDERLESS SUPPORT)
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if ((int)m.Result == HTCLIENT)
                {
                    Point cursor = PointToClient(Cursor.Position);

                    IntPtr hit = ResizeHitTestHelper.GetHitTest(this, cursor, 8);
                    if (hit != IntPtr.Zero)
                    {
                        m.Result = hit;
                        return;
                    }
                }
                return;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region THEMES
        private void UpdateThemeDropdownText()
        {
            poisonDropDownButton1_selectTheme.Text = ThemeHelper.GetTheme().ToString();
        }
        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeHelper.SaveTheme(ThemeStyle.Default);
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            _mainForm.ThemeChanged();
            UpdateThemeDropdownText();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeHelper.SaveTheme(ThemeStyle.Light);
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            _mainForm.ThemeChanged();
            UpdateThemeDropdownText();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeHelper.SaveTheme(ThemeStyle.Dark);
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            _mainForm.ThemeChanged();
            UpdateThemeDropdownText();
        }
        #endregion

        private void poisonDropDownButton1_selectTheme_Click(object sender, EventArgs e)
        {
            poisonDropDownButton1_selectTheme.OpenDropDown();
        }

        private void poisonToggle1_quickPaste_CheckedChanged(object sender, EventArgs e)
        {
            SettingsLoader.Current.QuickPasteOnEnter = poisonToggle1_quickPaste.Checked;
            SettingsLoader.Save();
            _mainForm.ApplyFormBehaviorSettings(true);
        }
    }
}
