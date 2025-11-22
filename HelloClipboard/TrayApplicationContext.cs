using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
		private bool _trayMinimizedNotifyShown;
		public bool ApplicationExiting;
		public static TrayApplicationContext Instance { get; private set; }
		private bool _suppressClipboardEvents = false;

		public TrayApplicationContext()
		{
			Instance = this;

			_form = new MainForm(this);

			if (!_form.IsHandleCreated)
			{
				var handle = _form.Handle;
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

			_trayIcon.ContextMenuStrip = trayMenu;

			_trayIcon.DoubleClick += (s, e) =>
			{
				ShowMainWindow();
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

			if (SettingsLoader.Current.CheckUpdates)
			{
				StartAutoUpdateCheck();
			}

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

			HideMainWindow();
			ShowMainWindow();

			_form.ResetFormPositionAndSize();
		}
		#region CLIPBOARD HANDLING

		public void SuppressClipboardEvents(bool value)
		{
			_suppressClipboardEvents = value;
		}

		private void OnClipboardUpdate(object sender, EventArgs e)
		{
			if (_suppressClipboardEvents)
				return;
			try
			{
				if (Clipboard.ContainsText())
				{
					string text = Clipboard.GetText();
					AddToCache(text);
				}
				else if (Clipboard.ContainsFileDropList())
				{
					var files = Clipboard.GetFileDropList();
					AddToCache(string.Join(", ", files.Cast<string>()));
				}
				else if (Clipboard.ContainsImage())
				{
					var image = Clipboard.GetImage();
					AddToCache($"[IMAGE {_clipboardCache.Count}]", image);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				MessageBox.Show($"Clipboard update error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
			}
		}

		private void AddToCache(string content, Image image = null)
		{
			if (string.IsNullOrWhiteSpace(content) && image == null)
				return;

			if (image != null)
			{
				if (_clipboardCache.Any())
				{
					var lastItem = _clipboardCache.Last();

					if (lastItem.ImageContent != null)
					{
						if (ImageAnalizer.AreImagesEqual(image, lastItem.ImageContent))
						{
							return;
						}
					}
				}
			}

			if (SettingsLoader.Current.PreventClipboardDuplication && image == null)
			{
				var existingItems = _clipboardCache.Where(cacheItem => cacheItem.Content == content).ToList();

				if (existingItems.Any())
				{
					var itemToKeep = existingItems.Last();
					foreach (var i in existingItems)
					{
						_clipboardCache.Remove(i);
						if (!_form.IsDisposed)
						{
							_form.RemoveItem(i);
						}
					}
					_clipboardCache.Add(itemToKeep);
					if (!_form.IsDisposed)
					{
						_form.MessageWriteLine(itemToKeep);
					}
					return;
				}
			}

			string cleanedContent = content;
			if (SettingsLoader.Current.EnableBetterHistoryVisualization)
			{
				string replacedContent = content.Replace('\r', ' ')
												.Replace('\n', ' ')
												.Replace('\t', ' ');
				cleanedContent = Regex.Replace(replacedContent, @"\s+", " ");
			}

			ClipboardItemType type = image != null ? ClipboardItemType.Image : ClipboardItemType.Text;
			var item = new ClipboardItem(_clipboardCache.Count,type, content, cleanedContent, image);
			_clipboardCache.Add(item);

			if (!_form.IsDisposed)
			{
				_form.MessageWriteLine(item);
			}

			if (_clipboardCache.Count > SettingsLoader.Current.MaxHistoryCount)
			{
				_form.RemoveAt(0);
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

					if (last == default || (now - last) >= Constants.applicationUpdateInterval)
					{
						await DoUpdateCheck();
						TempConfigLoader.Current.LastUpdateCheck = DateTime.UtcNow;
						TempConfigLoader.Save();
					}

					var remaining = Constants.applicationUpdateInterval - (now - last);
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
			_form.ShowInTaskbar = true;

			_form.Activate();
			_form.BringToFront();

			_form.FocusSearchBox();
		}

		public void HideMainWindow()
		{
			_form?.Hide();
			_form?.CloseDetailFormIfAvaible();
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
			_trayIcon.Visible = false;
			_trayIcon.Dispose();
			ExitThread();
		}

		#endregion
	}
}
