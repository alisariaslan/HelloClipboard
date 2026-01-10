using HelloClipboard.Services;
using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HelloClipboard
{
	/// <summary>
	/// Manages the application lifecycle, background services, and system tray integration.
	/// </summary>
	public class TrayApplicationContext : ApplicationContext
	{
		public static TrayApplicationContext Instance { get; private set; }
		public bool ApplicationExiting { get; private set; }

		private MainForm _form;
		private TrayIconManager _trayManager;
		private bool _trayMinimizedNotifyShown;

		private readonly HistoryHelper _historyHelper = new HistoryHelper();
		private readonly ClipboardMonitor _clipboardMonitor;
		private readonly HotkeyService _hotkeyService = new HotkeyService();
		private readonly PrivacyService _privacyService = new PrivacyService();
		private readonly UpdateService _updateService = new UpdateService();

		public int GetStoredCount() => _historyHelper.GetStoredItemCount();

		public TrayApplicationContext()
		{
			Instance = this;

			// 1. Initialize Services
			_clipboardMonitor = new ClipboardMonitor(_historyHelper, _privacyService);
			BindEvents();
			_clipboardMonitor.Start();

			// 2. Prepare UI Components
			_form = new MainForm(this);
			var forceHandleCreation = _form.Handle;
			_trayManager = new TrayIconManager(
				onShow: ShowMainWindow,
				onHide: HideMainWindow,
				onReset: ResetFormPositionAndSize,
				onTogglePrivacy: TogglePrivacyMode,
				onExit: ExitApplication
			);

			// 3. Apply Settings
			if (SettingsLoader.Current.CheckUpdates)
				_updateService.StartPeriodicCheck(Application.ProductVersion);

			TryRegisterGlobalHotkey();
			UpdatePrivacyMenuText();
			HandleInitialVisibility();
		}

		#region Initialization & Binding

		private void BindEvents()
		{
			// Clipboard Events
			_clipboardMonitor.ItemCaptured += (item) => RunOnUI(() => _form.MessageAdd(item));
			_clipboardMonitor.ItemUpdated += (item) => RunOnUI(() => {
				_form.MessageRemoveItem(item);
				_form.MessageAdd(item);
			});
			_clipboardMonitor.ItemRemoved += () => RunOnUI(() => _form.RemoveOldestMessage());
			_clipboardMonitor.ClipboardCleared += () => RunOnUI(() => {
				_form.RefreshCacheView();
				_form.ClearSearchBox();
			});

			// Service Events
			_updateService.UpdateAvailable += OnUpdateAvailable;
			_privacyService.StateChanged += OnPrivacyStateChanged;
			_privacyService.Tick += UpdatePrivacyMenuText;
		}

		private void HandleInitialVisibility()
		{
			// Logic to determine if the app should start minimized or visible
			if (SettingsLoader.Current.HideToTray && !TempConfigLoader.Current.AdminPriviligesRequested)
				HideMainWindow();
			else
				ShowMainWindow();

			TempConfigLoader.Current.AdminPriviligesRequested = false;
			TempConfigLoader.Save();
		}

		#endregion

		#region Window Management

		public void ShowMainWindow()
		{
			RunOnUI(() => {
				if (_form.IsDisposed) _form = new MainForm(this);
				_form.Show();
				_form.WindowState = FormWindowState.Normal;
				_form.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;
				_form.Activate();
				_form.BringToFront();
				_form.FocusSearchBox();
			});
		}

		public void HideMainWindow()
		{
			if (_form == null) return;
			RunOnUI(() => {
				_form.CloseDetailFormIfAvaible();
				// Close any child/owned forms before hiding the main window
				foreach (Form owned in _form.OwnedForms)
					if (owned != null && !owned.IsDisposed) try { owned.Close(); } catch { }

				_form.Hide();
				if (!_trayMinimizedNotifyShown)
				{
					_trayManager.ShowNotification(Constants.AppName, "Minimized to tray.");
					_trayMinimizedNotifyShown = true;
				}
			});
		}

		private void ResetFormPositionAndSize()
		{
			var cfg = TempConfigLoader.Current;
			cfg.MainFormWidth = cfg.MainFormHeight = cfg.MainFormX = cfg.MainFormY = -1;
			TempConfigLoader.Save();
			RunOnUI(() => {
				_form.ResetFormPositionAndSize();
				HideMainWindow();
				ShowMainWindow();
				ScreenHelper.CenterFormManually(_form);
			});
		}

		#endregion

		#region Logic Handlers

		public void ClearClipboard()
		{
			_clipboardMonitor.SuppressEvents(true);
			try
			{
				Clipboard.Clear();

				_clipboardMonitor.ClearAll();

				int legacyCount = TempConfigLoader.Current.PinnedHashes.RemoveAll(h => !h.Contains("_"));

				if (legacyCount > 0)
				{
					TempConfigLoader.Save();
				}

				MessageBox.Show($"Clipboard cleared and {legacyCount} legacy pin records cleaned.", "Success", MessageBoxButtons.OK);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_clipboardMonitor.SuppressEvents(false);
			}
		}

		public void TogglePrivacyMode() => _privacyService.Toggle(_privacyService.GetPrivacyDurationMinutes());
		public bool IsPrivacyModeActive => _privacyService.IsActive;

		private void OnPrivacyStateChanged(bool active)
		{
			UpdatePrivacyMenuText();
			_trayManager.ShowNotification(Constants.AppName, $"Private Mode {(active ? "enabled" : "disabled")}.");
		}

		private void UpdatePrivacyMenuText()
		{
			RunOnUI(() => {
				var remaining = Math.Max(0, Math.Ceiling((_privacyService.Until - DateTime.UtcNow).TotalMinutes));
				_trayManager.UpdatePrivacyText(_privacyService.IsActive, remaining, _privacyService.GetPrivacyDurationMinutes());
			});
		}

		private void OnUpdateAvailable(object sender, UpdateInfo e)
		{
			RunOnUI(() => _form.UpdateCheckUpdateNowBtnText("Update Now"));
			_trayManager.ShowNotification($"{Constants.AppName} Update", $"Version v{e.Version} available.", ToolTipIcon.Info, ShowMainWindow);
		}

		#endregion

		#region Global Hotkey

		public bool ReloadGlobalHotkey() => TryRegisterGlobalHotkey();

		private bool TryRegisterGlobalHotkey()
		{
			if (!SettingsLoader.Current.EnableGlobalHotkey || SettingsLoader.Current.HotkeyKey == Keys.None)
				return false;

			_hotkeyService.HotkeyPressed -= ToggleMainWindowFromHotkey;
			_hotkeyService.HotkeyPressed += ToggleMainWindowFromHotkey;
			return _hotkeyService.Register(SettingsLoader.Current.HotkeyModifiers, SettingsLoader.Current.HotkeyKey);
		}

		private void ToggleMainWindowFromHotkey()
		{
			if (ApplicationExiting) return;
			if (_form.Visible) HideMainWindow(); else ShowMainWindow();
		}

		#endregion

		#region Lifecycle & Helpers

		public void ExitApplication()
		{
			if (ApplicationExiting) return;
			ApplicationExiting = true;

			// Cleanup services and resources
			_updateService.StopPeriodicCheck();
			_hotkeyService.Dispose();
			_privacyService.Disable();
			_trayManager.Dispose();

			if (_form != null && !_form.IsDisposed)
			{
				_form.Close();
				_form.Dispose();
			}
			ExitThread();
		}


		/// <summary>
		/// Ensures the given action is executed on the UI thread.
		/// </summary>
		private void RunOnUI(Action action)
		{
			if (_form == null || _form.IsDisposed) return;

			if (!_form.IsHandleCreated)
			{
				IntPtr dummy = _form.Handle;
			}

			if (_form.InvokeRequired)
			{
				_form.BeginInvoke(action);
			}
			else
			{
				action();
			}
		}

		// Public Getters
		public UpdateService GetUpdateService() => _updateService;
		public IReadOnlyList<ClipboardItem> GetClipboardCache() => _clipboardMonitor.GetCache();
		public void SuppressClipboardEvents(bool value) => _clipboardMonitor.SuppressEvents(value);
		public void RefreshPrivacyMenuLabel() => UpdatePrivacyMenuText();
		public void RequestDeletion(ClipboardItem item)
		{
			_clipboardMonitor.RemoveItem(item);
		}

		#endregion
	}
}