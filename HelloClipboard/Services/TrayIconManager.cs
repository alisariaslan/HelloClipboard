using System;
using System.Windows.Forms;

namespace HelloClipboard
{
	/// <summary>
	/// Manages the system tray icon (NotifyIcon), including the context menu, 
	/// notifications, and user interaction events.
	/// </summary>
	public class TrayIconManager : IDisposable
	{
		private readonly NotifyIcon _trayIcon;
		private readonly ToolStripMenuItem _privacyMenuItem;

		public TrayIconManager(Action onShow, Action onHide, Action onReset, Action onTogglePrivacy, Action onExit)
		{
			_privacyMenuItem = new ToolStripMenuItem(string.Empty, null, (s, e) => onTogglePrivacy());

			var trayMenu = new ContextMenuStrip();
			trayMenu.Items.Add("Show", null, (s, e) => onShow());
			trayMenu.Items.Add("Hide", null, (s, e) => onHide());
			trayMenu.Items.Add(new ToolStripMenuItem("Reset Window", null, (s, e) => onReset()));
			trayMenu.Items.Add(_privacyMenuItem);
			trayMenu.Items.Add("-");
			trayMenu.Items.Add("Exit", null, (s, e) => onExit());
			trayMenu.Items.Add("Cancel", null, (s, e) => trayMenu.Close());

			_trayIcon = new NotifyIcon
			{
				Icon = Properties.Resources.favicon,
				Visible = true,
				Text = Constants.AppName,
				ContextMenuStrip = trayMenu
			};


			_trayIcon.MouseUp += (s, e) => {
				if (e.Button == MouseButtons.Right)
				{
					System.Drawing.Size menuSize = trayMenu.PreferredSize;

					var workingArea = Screen.PrimaryScreen.WorkingArea;

					int x = Cursor.Position.X - (menuSize.Width / 2);
					if (x + menuSize.Width > workingArea.Right) x = workingArea.Right - menuSize.Width;
					if (x < workingArea.Left) x = workingArea.Left;

					int y = workingArea.Bottom - menuSize.Height;

					if (y < workingArea.Top) y = workingArea.Top;

					trayMenu.Show(new System.Drawing.Point(x, y));
				}
			};

			_trayIcon.MouseClick += (s, e) => {
				if (e.Button == MouseButtons.Left)
					onShow();
			};
		}

		/// <summary>
		/// Displays a balloon tip notification from the tray icon.
		/// </summary>
		/// <param name="title">The title of the notification.</param>
		/// <param name="text">The body content of the notification.</param>
		/// <param name="icon">The type of icon to display (Info, Warning, Error).</param>
		/// <param name="onClick">Optional callback action when the notification is clicked.</param>
		public void ShowNotification(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, Action onClick = null)
		{
			_trayIcon.ShowBalloonTip(3000, title, text, icon);

			if (onClick != null)
			{
				EventHandler handler = null;
				handler = (s, e) => {
					onClick();
					// Unsubscribe to prevent memory leaks and multiple executions
					_trayIcon.BalloonTipClicked -= handler;
				};
				_trayIcon.BalloonTipClicked += handler;
			}
		}

		/// <summary>
		/// Updates the label of the Privacy Mode menu item based on the current state.
		/// </summary>
		/// <param name="isActive">Indicates if Privacy Mode is currently enabled.</param>
		/// <param name="remainingMin">The remaining minutes before Privacy Mode auto-disables.</param>
		/// <param name="defaultMin">The default duration defined in settings.</param>
		public void UpdatePrivacyText(bool isActive, double remainingMin, int defaultMin)
		{
			_privacyMenuItem.Text = isActive
				? $"Disable Private Mode ({remainingMin} min left)"
				: $"Enable Private Mode ({defaultMin} min)";
		}

		/// <summary>
		/// Properly hides and releases resources associated with the tray icon.
		/// </summary>
		public void Dispose()
		{
			_trayIcon.Visible = false;
			_trayIcon.Dispose();
		}
	}
}