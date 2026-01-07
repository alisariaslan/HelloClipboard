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
			// Initialize the privacy menu item as a class field to update its text dynamically
			_privacyMenuItem = new ToolStripMenuItem(string.Empty, null, (s, e) => onTogglePrivacy());

			var trayMenu = new ContextMenuStrip();
			trayMenu.Items.Add("Show", null, (s, e) => onShow());
			trayMenu.Items.Add("Hide", null, (s, e) => onHide());
			trayMenu.Items.Add(new ToolStripMenuItem("Reset Window", null, (s, e) => onReset()));
			trayMenu.Items.Add(_privacyMenuItem);
			trayMenu.Items.Add("-"); // Separator
			trayMenu.Items.Add("Exit", null, (s, e) => onExit());

			_trayIcon = new NotifyIcon
			{
				Icon = Properties.Resources.favicon,
				Visible = true,
				Text = Constants.AppName,
				ContextMenuStrip = trayMenu
			};

			// Handle Double-Click to show the main window
			_trayIcon.DoubleClick += (s, e) => onShow();

			// Handle Single-Click based on user preferences
			_trayIcon.MouseClick += (s, e) => {
				if (e.Button == MouseButtons.Left && SettingsLoader.Current.OpenWithSingleClick)
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