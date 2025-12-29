using HelloClipboard.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
		private readonly HashSet<string> _clipboardHashPool = new HashSet<string>();
		private bool _trayMinimizedNotifyShown;
		public bool ApplicationExiting;
		public static TrayApplicationContext Instance { get; private set; }
		private bool _suppressClipboardEvents = false;
		private HistoryHelper historyHelper { get; set; }

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

		private void OnClipboardUpdate(object sender, EventArgs e)
		{
			if (_suppressClipboardEvents)
				return;
			try
			{
				if (Clipboard.ContainsText())
				{
					string text = Clipboard.GetText();
					AddToCache(ClipboardItemType.Text, text);
				}
				else if (Clipboard.ContainsFileDropList())
				{
					var files = Clipboard.GetFileDropList();
					foreach (var file in files)
					{
						AddToCache(ClipboardItemType.File, file);
					}
				}
				else if (Clipboard.ContainsImage())
				{
					var image = Clipboard.GetImage();
					var imageCount = _clipboardCache.Count(i => i.ItemType == ClipboardItemType.Image);
					AddToCache(ClipboardItemType.Image, $"[IMAGE {imageCount + 1}]", image);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				MessageBox.Show($"Clipboard update error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
			}
		}

		private void AddToCache(ClipboardItemType type, string textContent, Image imageContent = null)
		{
			if (string.IsNullOrWhiteSpace(textContent) && imageContent == null)
				return;

			string calculatedHash = null;
			ClipboardItem existingItem = null;

			if (SettingsLoader.Current.PreventClipboardDuplication)
			{
				if (type == ClipboardItemType.Text || type == ClipboardItemType.File)
				{
					calculatedHash = HashHelper.CalculateMd5Hash(textContent);
				}
				else if (type == ClipboardItemType.Image && imageContent != null)
				{
					calculatedHash = HashHelper.HashImageBytes(imageContent);
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

			if (SettingsLoader.Current.EnableClipboardHistory && item.ContentHash != null)
			{
				historyHelper.SaveItemToHistoryFile(item);
			}

			_clipboardCache.Add(item);

			if (item.ContentHash != null)
			{
				_clipboardHashPool.Add(item.ContentHash);
			}

			if (!_form.IsDisposed)
			{
				_form.MessageAdd(item);
			}

			if (_clipboardCache.Count > SettingsLoader.Current.MaxHistoryCount)
			{
				var oldestItem = _clipboardCache[0];

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
