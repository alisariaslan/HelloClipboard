using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public class TrayApplicationContext : ApplicationContext
	{
		private NotifyIcon _trayIcon;
		private MainForm _form;
		private bool _updateChecksStarted;
		private readonly List<ClipboardItem> _clipboardCache = new List<ClipboardItem>();
		private readonly HashSet<string> _clipboardHashPool = new HashSet<string>();
		private bool _trayMinimizedNotifyShown;
		public bool ApplicationExiting;
		public static TrayApplicationContext Instance { get; private set; }
		private bool _suppressClipboardEvents = false;
		private HistoryHelper historyHelper { get; set; }
		private uint _lastClipboardSequenceNumber;
		private string _lastTextContent;
		private string _lastFileContent;
		private string _lastImageHash;
		private HotkeyWindow _hotkeyWindow;
		private bool _hotkeyRegistered;
		public bool HotkeyRegistered => _hotkeyRegistered;
		private bool _privacyModeActive;
		private DateTime _privacyModeUntil;
		private ToolStripMenuItem _trayPrivacyMenuItem;

		private const int ClipboardMaxAttempts = 4;
		private const int ClipboardFastRetryDelayMs = 25;
		private const int HotkeyId = 1001;

		internal const int WM_HOTKEY = 0x0312;
		private const uint MOD_ALT = 0x0001;
		private const uint MOD_CONTROL = 0x0002;
		private const uint MOD_SHIFT = 0x0004;
		private const uint MOD_WIN = 0x0008;

		[DllImport("user32.dll")]
		private static extern uint GetClipboardSequenceNumber();
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public TrayApplicationContext()
		{
			Instance = this;

			_form = new MainForm(this);
			if (!_form.IsHandleCreated)
			{
				var handle = _form.Handle;
			}

			historyHelper = new HistoryHelper();
			if (SettingsLoader.Current.EnableClipboardHistory)
			{
				var loadedItems = historyHelper.LoadHistoryFromFiles();
				foreach (var item in loadedItems)
				{
					if (item.ContentHash != null && TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
					{
						item.IsPinned = true;
					}
					_clipboardCache.Add(item);
					if (item.ContentHash != null)
					{
						_clipboardHashPool.Add(item.ContentHash);
					}
				}

#if DEBUG
				string historyPath = Constants.HistoryDirectory;
				if (Directory.Exists(historyPath))
				{
					try
					{
						//Process.Start(historyPath);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine($"Error: History folder couldn't open: {ex.Message}");
					}
				}
#endif
			}

			_trayIcon = new NotifyIcon()
			{
				Icon = Properties.Resources.favicon,
				Visible = true,
				Text = $"{Constants.AppName}"
			};
			var trayMenu = new ContextMenuStrip();
			trayMenu.Items.Add("Show", null, (s, e) => ShowMainWindow());
			trayMenu.Items.Add("Exit", null, (s, e) => ExitApplication());
			trayMenu.Items.Add(new ToolStripMenuItem("Reset Window", null, (s, e) => ResetFormPositionAndSize()));
			_trayPrivacyMenuItem = new ToolStripMenuItem($"Enable Private Mode ({GetPrivacyDurationMinutes()} min)", null, (s, e) => TogglePrivacyMode());
			trayMenu.Items.Add(_trayPrivacyMenuItem);
			_trayIcon.ContextMenuStrip = trayMenu;
			_trayIcon.DoubleClick += (s, e) =>
			{
				ShowMainWindow();
			};
			_trayIcon.MouseClick += (s, e) =>
			{
				if (e.Button == MouseButtons.Left && SettingsLoader.Current.OpenWithSingleClick)
				{
					ShowMainWindow();
				}
			};
			if (SettingsLoader.Current.HideToTray && !TempConfigLoader.Current.AdminPriviligesRequested)
			{
				HideMainWindow();
			}
			else
			{
				ShowMainWindow();
			}
			if (SettingsLoader.Current.CheckUpdates)
			{
				StartAutoUpdateCheck();
			}

			TryRegisterGlobalHotkey();

			TempConfigLoader.Current.AdminPriviligesRequested = false;
			TempConfigLoader.Save();
			ClipboardNotification.ClipboardUpdate += OnClipboardUpdate;
		}

		private void ResetFormPositionAndSize()
		{
			if (_form == null || _form.IsDisposed)
				return;
			var cfg = TempConfigLoader.Current;
			cfg.MainFormWidth = -1;
			cfg.MainFormHeight = -1;
			cfg.MainFormX = -1;
			cfg.MainFormY = -1;
			TempConfigLoader.Save();
			_form.ResetFormPositionAndSize();
			HideMainWindow();
			ShowMainWindow();
			ScreenHelper.CenterFormManually(_form);
		}

		#region CLIPBOARD HANDLING

		public void ClearClipboard()
		{
			SuppressClipboardEvents(true);

			try
			{
				Clipboard.Clear();

				_clipboardCache.Clear();
				_clipboardHashPool.Clear();
				TempConfigLoader.Current.PinnedHashes.Clear();

				if (SettingsLoader.Current.EnableClipboardHistory)
				{
					try
					{
						string historyPath = Constants.HistoryDirectory;
						if (Directory.Exists(historyPath))
						{
							var files = Directory.GetFiles(historyPath);
							foreach (var file in files)
							{
								File.Delete(file);
							}
						}
					}
					catch (Exception ex)
					{
#if DEBUG
						System.Diagnostics.Debug.WriteLine($"History delete error: {ex.Message}");
#endif
					}
				}

				if (_form != null && !_form.IsDisposed)
				{
					_form.Invoke(new MethodInvoker(() =>
					{
						_form.RefreshCacheView();
						_form.ClearSearchBox();
					}));
				}

				MessageBox.Show($"Clipboard cleared.", "Success", MessageBoxButtons.OK);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error clearing clipboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				SuppressClipboardEvents(false);
			}
		}

		public void SuppressClipboardEvents(bool value)
		{
			_suppressClipboardEvents = value;
		}

		public bool ReloadGlobalHotkey()
		{
			UnregisterGlobalHotkey();
			return TryRegisterGlobalHotkey();
		}

		private bool TryRegisterGlobalHotkey()
		{
			if (!SettingsLoader.Current.EnableGlobalHotkey)
				return false;

			if (SettingsLoader.Current.HotkeyKey == Keys.None)
				return false;

			if (_hotkeyWindow == null)
			{
				_hotkeyWindow = new HotkeyWindow(ToggleMainWindowFromHotkey);
			}

			UnregisterGlobalHotkey();

			uint modifiers = BuildModifierFlags(SettingsLoader.Current.HotkeyModifiers);
			uint key = (uint)SettingsLoader.Current.HotkeyKey;

			bool ok = RegisterHotKey(_hotkeyWindow.Handle, HotkeyId, modifiers, key);
			_hotkeyRegistered = ok;
#if DEBUG
			if (!ok)
			{
				System.Diagnostics.Debug.WriteLine("Global hotkey registration failed.");
			}
#endif
			return ok;
		}

		private void UnregisterGlobalHotkey()
		{
			if (_hotkeyWindow == null || !_hotkeyRegistered)
				return;
			try
			{
				UnregisterHotKey(_hotkeyWindow.Handle, HotkeyId);
			}
			catch { }
			_hotkeyRegistered = false;
		}

		private uint BuildModifierFlags(Keys modifiers)
		{
			uint mods = 0;
			if (modifiers.HasFlag(Keys.Control)) mods |= MOD_CONTROL;
			if (modifiers.HasFlag(Keys.Alt)) mods |= MOD_ALT;
			if (modifiers.HasFlag(Keys.Shift)) mods |= MOD_SHIFT;
			if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin)) mods |= MOD_WIN;
			return mods;
		}

		private void TogglePrivacyMode()
		{
			if (_privacyModeActive)
			{
				DisablePrivacyMode();
			}
			else
			{
				EnablePrivacyMode(GetPrivacyDuration());
			}
		}

		private void EnablePrivacyMode(TimeSpan duration)
		{
			_privacyModeActive = true;
			_privacyModeUntil = DateTime.UtcNow.Add(duration);
			UpdatePrivacyMenuText();
			_trayIcon.ShowBalloonTip(2000, $"{Constants.AppName}", $"Private Mode enabled for {duration.TotalMinutes} minutes.", ToolTipIcon.Info);
			Task.Run(async () =>
			{
				while (_privacyModeActive && DateTime.UtcNow < _privacyModeUntil)
				{
					await Task.Delay(TimeSpan.FromSeconds(10));
				}
				if (_privacyModeActive && DateTime.UtcNow >= _privacyModeUntil)
				{
					DisablePrivacyMode();
				}
			});
		}

		private void DisablePrivacyMode()
		{
			_privacyModeActive = false;
			UpdatePrivacyMenuText();
			_trayIcon.ShowBalloonTip(2000, $"{Constants.AppName}", "Private Mode disabled.", ToolTipIcon.Info);
		}

		private void UpdatePrivacyMenuText()
		{
			if (_trayPrivacyMenuItem == null)
				return;
			if (_privacyModeActive)
			{
				var remaining = _privacyModeUntil - DateTime.UtcNow;
				if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
				_trayPrivacyMenuItem.Text = $"Disable Private Mode ({Math.Ceiling(remaining.TotalMinutes)} min left)";
			}
			else
			{
				_trayPrivacyMenuItem.Text = $"Enable Private Mode ({GetPrivacyDurationMinutes()} min)";
			}
		}

		private void ToggleMainWindowFromHotkey()
		{
			if (ApplicationExiting)
				return;

			if (_form != null && !_form.IsDisposed && _form.Visible)
			{
				HideMainWindow();
			}
			else
			{
				ShowMainWindow();
			}
		}

		private async void OnClipboardUpdate(object sender, EventArgs e)
		{
			if (_suppressClipboardEvents)
				return;

			if (_privacyModeActive)
			{
				if (DateTime.UtcNow >= _privacyModeUntil)
				{
					DisablePrivacyMode();
				}
				else
				{
					return;
				}
			}

			// Yinelenen olayları atla
			uint seq = GetClipboardSequenceNumber();
			if (seq != 0 && seq == _lastClipboardSequenceNumber)
				return;
			_lastClipboardSequenceNumber = seq;

			await TryReadClipboardAsync();
		}

		private async Task TryReadClipboardAsync()
		{
			for (int attempt = 0; attempt < ClipboardMaxAttempts; attempt++)
			{
				try
				{
					var dataObj = Clipboard.GetDataObject();

					if (dataObj != null && dataObj.GetDataPresent(DataFormats.UnicodeText, true))
					{
						string text = dataObj.GetData(DataFormats.UnicodeText, true) as string;
						if (!string.IsNullOrEmpty(text))
						{
							AddToCache(ClipboardItemType.Text, text);
							return;
						}
					}

					if (dataObj != null && dataObj.GetDataPresent(DataFormats.FileDrop))
					{
						var files = dataObj.GetData(DataFormats.FileDrop) as string[];
						if (files != null && files.Length > 0)
						{
							foreach (var file in files)
							{
								AddToCache(ClipboardItemType.File, file);
							}
							return;
						}
					}

					if (dataObj != null && dataObj.GetDataPresent(DataFormats.Bitmap))
					{
						var image = Clipboard.GetImage();
						if (image != null)
						{
							var imageCount = _clipboardCache.Count(i => i.ItemType == ClipboardItemType.Image);
							AddToCache(ClipboardItemType.Image, $"[IMAGE {imageCount + 1}]", image);
							return;
						}
					}

					// Pano hazır değilse kısa bir beklemeyle tekrar dene
					if (attempt < ClipboardMaxAttempts - 1)
						await Task.Delay(ClipboardFastRetryDelayMs);
				}
				catch (ExternalException)
				{
					if (attempt < ClipboardMaxAttempts - 1)
						await Task.Delay(ClipboardFastRetryDelayMs);
				}
				catch (Exception ex)
				{
#if DEBUG
					System.Diagnostics.Debug.WriteLine($"Clipboard Error: {ex.Message}");
#endif
					return;
				}
			}
		}

		private void AddToCache(ClipboardItemType type, string textContent, Image imageContent = null)
		{
			if (string.IsNullOrWhiteSpace(textContent) && imageContent == null)
				return;

			bool preventDuplication = SettingsLoader.Current.PreventClipboardDuplication;

			if (preventDuplication)
			{
				if (type == ClipboardItemType.Text && textContent == _lastTextContent)
					return;

				if (type == ClipboardItemType.File && textContent == _lastFileContent)
					return;
			}

			string imageHash = null;
			if (type == ClipboardItemType.Image && imageContent != null)
			{
				imageHash = HashHelper.HashImageBytes(imageContent);
				if (preventDuplication && !string.IsNullOrEmpty(imageHash) && imageHash == _lastImageHash)
					return;
				_lastImageHash = imageHash;
			}

			string calculatedHash = null;
			ClipboardItem existingItem = null;

			if (preventDuplication)
			{
				if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
				{
					calculatedHash = HashHelper.CalculateMd5Hash(textContent);
				}
				else if (type == ClipboardItemType.Image && imageContent != null)
				{
					calculatedHash = imageHash ?? HashHelper.HashImageBytes(imageContent);
				}
			}

			if (calculatedHash != null && _clipboardHashPool.Contains(calculatedHash))
			{
				existingItem = _clipboardCache.FirstOrDefault(i => i.ContentHash == calculatedHash);

				if (existingItem != null)
				{
					_clipboardCache.Remove(existingItem);
					if (!_form.IsDisposed) { _form.MessageRemoveItem(existingItem); }

					_clipboardCache.Add(existingItem);
					if (!_form.IsDisposed) { _form.MessageAdd(existingItem); }
				}
				return;
			}

			string newTitle = "";

			if ( type == ClipboardItemType.Text)
			{
				string replacedNewlines = textContent.Replace('\r', ' ')
										 .Replace('\n', ' ')
										 .Replace('\t', ' ');
				string cleanedWhitespace = Regex.Replace(replacedNewlines, @"\s+", " ");
				newTitle = cleanedWhitespace.Trim();
				if (newTitle.Length > 1024)
				{
					newTitle = newTitle.Substring(0, 1024) + "...";
				}
			}
			else if ( type == ClipboardItemType.Image)
			{
				newTitle = textContent;
			}
			else if ( type == ClipboardItemType.File)
			{
				newTitle = $"{System.IO.Path.GetFileName(textContent)} -> {textContent}";
			}

			var item = new ClipboardItem(_clipboardCache.Count, type, textContent, newTitle, imageContent, calculatedHash);
			if (item.ContentHash != null && TempConfigLoader.Current.PinnedHashes.Contains(item.ContentHash))
			{
				item.IsPinned = true;
			}

			if (SettingsLoader.Current.EnableClipboardHistory && item.ContentHash != null)
			{
				// Disk yazımını UI işleyicisinden ayır
				Task.Run(() => historyHelper.SaveItemToHistoryFile(item));
			}

			if (item.IsPinned)
			{
				_clipboardCache.Add(item);
			}
			else
			{
				var insertIndex = _clipboardCache.TakeWhile(i => i.IsPinned).Count();
				_clipboardCache.Insert(insertIndex, item);
			}

			if (item.ContentHash != null)
			{
				_clipboardHashPool.Add(item.ContentHash);
			}

			if (type == ClipboardItemType.Text)
				_lastTextContent = textContent;
			else if (type == ClipboardItemType.File)
				_lastFileContent = textContent;

			if (!_form.IsDisposed)
			{
				_form.MessageAdd(item);
			}

			if (_clipboardCache.Count > SettingsLoader.Current.MaxHistoryCount)
			{
				var oldestItem = _clipboardCache[0];

				if (oldestItem.IsPinned)
				{
					// Pinned kalemleri atlamadan en eski pinlenmeyen öğeyi bul
					var removable = _clipboardCache.FirstOrDefault(i => !i.IsPinned);
					if (removable == null)
						return;
					oldestItem = removable;
				}

				if (oldestItem.ContentHash != null)
				{
					_clipboardHashPool.Remove(oldestItem.ContentHash);

					if (SettingsLoader.Current.EnableClipboardHistory)
					{
						historyHelper.DeleteItemFromFile(oldestItem.ContentHash);
					}
				}

				_form.RemoveOldestMessage();
				_clipboardCache.RemoveAt(0);
			}
		}

		public IReadOnlyList<ClipboardItem> GetClipboardCache()
		{
			return _clipboardCache.AsReadOnly();
		}

		#endregion

		public async void StartAutoUpdateCheck()
		{
			if (_updateChecksStarted)
				return;
			_updateChecksStarted = true;
			while (SettingsLoader.Current.CheckUpdates)
			{
				try
				{
					var now = DateTime.UtcNow;
					var last = TempConfigLoader.Current.LastUpdateCheck;
					if (last == default || (now - last) >= Constants.ApplicationUpdateInterval)
					{
						await DoUpdateCheck();
						TempConfigLoader.Current.LastUpdateCheck = DateTime.UtcNow;
						TempConfigLoader.Save();
					}
					var remaining = Constants.ApplicationUpdateInterval - (now - last);
					if (remaining < TimeSpan.Zero)
						remaining = TimeSpan.Zero;
					await Task.Delay(remaining);
				}
				catch
				{
				}
			}
			_updateChecksStarted = false;
		}

		private async Task DoUpdateCheck()
		{
			var update = await UpdateService.CheckForUpdateAsync(Application.ProductVersion, true);
			if (update != null)
			{
				if (!_form.IsDisposed)
					_form.UpdateCheckUpdateNowBtnText("Update Now");
				_trayIcon.BalloonTipTitle = $"{Constants.AppName} Update";
				_trayIcon.BalloonTipText = "A new version is available. Click \"Update Now\".";
				_trayIcon.BalloonTipIcon = ToolTipIcon.Info;
				_trayIcon.ShowBalloonTip(5000);
				_trayIcon.BalloonTipClicked -= TrayIcon_BalloonTipClicked;
				_trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
			}
			else
			{
				if (!_form.IsDisposed)
					_form.UpdateCheckUpdateNowBtnText("Check Update");
			}
		}

		#region TRAY HANDLING
		private void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
		{
			ShowMainWindow();
		}

		public void ShowMainWindow()
		{
			if (_form.InvokeRequired)
			{
				_form.Invoke(new MethodInvoker(ShowMainWindow));
				return;
			}
			if (_form.IsDisposed)
			{
				_form = new MainForm(this);
				if (!_form.IsHandleCreated)
				{
					var handle = _form.Handle;
				}
			}
			_form.Show();
			_form.WindowState = FormWindowState.Normal;

			_form.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;

			_form.Activate();
			_form.BringToFront();
			_form.FocusSearchBox();
		}

		public void HideMainWindow()
		{
			if (_form == null)
				return;

			_form.CloseDetailFormIfAvaible();

			foreach (Form owned in _form.OwnedForms)
			{
				if (owned != null && !owned.IsDisposed)
				{
					try { owned.Close(); } catch { }
				}
			}

			_form.Hide();
			if (!_trayMinimizedNotifyShown)
			{
				_trayIcon.ShowBalloonTip(1000, $"{Constants.AppName}", "Application minimized to tray.", ToolTipIcon.Info);
				_trayMinimizedNotifyShown = true;
			}
		}

		public void ExitApplication()
		{
			if (ApplicationExiting)
				return;
			ApplicationExiting = true;
			if (_form != null && !_form.IsDisposed)
			{
				_form.Close();
				_form.Dispose();
			}
			UnregisterGlobalHotkey();
			_hotkeyWindow?.Dispose();
			_trayIcon.Visible = false;
			_trayIcon.Dispose();
			ExitThread();
		}

		public void RefreshPrivacyMenuLabel()
		{
			UpdatePrivacyMenuText();
		}

		private TimeSpan GetPrivacyDuration()
		{
			return TimeSpan.FromMinutes(GetPrivacyDurationMinutes());
		}

		private int GetPrivacyDurationMinutes()
		{
			int minutes = SettingsLoader.Current?.PrivacyModeDurationMinutes ?? 10;
			if (minutes < 1)
				minutes = 10;
			if (minutes > 99)
				minutes = 99;
			return minutes;
		}

		#endregion
	}

	internal class HotkeyWindow : NativeWindow, IDisposable
	{
		private readonly Action _onHotkey;

		public HotkeyWindow(Action onHotkey)
		{
			_onHotkey = onHotkey;
			CreateHandle(new CreateParams());
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == TrayApplicationContext.WM_HOTKEY)
			{
				_onHotkey?.Invoke();
			}
			base.WndProc(ref m);
		}

		public void Dispose()
		{
			DestroyHandle();
		}
	}
}
