using System;
using System.Windows.Forms;

namespace HelloClipboard
{
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

			_trayIcon = new NotifyIcon
			{
				Icon = Properties.Resources.favicon,
				Visible = true,
				Text = Constants.AppName,
				ContextMenuStrip = trayMenu
			};

			_trayIcon.DoubleClick += (s, e) => onShow();
			_trayIcon.MouseClick += (s, e) => {
				if (e.Button == MouseButtons.Left && SettingsLoader.Current.OpenWithSingleClick)
					onShow();
			};
		}

		public void ShowNotification(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, Action onClick = null)
		{
			_trayIcon.ShowBalloonTip(3000, title, text, icon);
			if (onClick != null)
			{
				EventHandler handler = null;
				handler = (s, e) => {
					onClick();
					_trayIcon.BalloonTipClicked -= handler;
				};
				_trayIcon.BalloonTipClicked += handler;
			}
		}

		public void UpdatePrivacyText(bool isActive, double remainingMin, int defaultMin)
		{
			_privacyMenuItem.Text = isActive
				? $"Disable Private Mode ({remainingMin} min left)"
				: $"Enable Private Mode ({defaultMin} min)";
		}

		public void Dispose()
		{
			_trayIcon.Visible = false;
			_trayIcon.Dispose();
		}
	}
}