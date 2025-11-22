using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class MainForm : Form
	{
		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainFormViewModel _viewModel;
		private bool _isLoaded = false;
		private ClipDetail _openDetailForm;

		public MainForm(TrayApplicationContext trayApplicationContext)
		{
			InitializeComponent();

			this.Text = Application.ProductName + " v" + Application.ProductVersion;

			_trayApplicationContext = trayApplicationContext;
			_viewModel = new MainFormViewModel(this);

			_viewModel.LoadSettings();

		}

		public void MessageWriteLine(string msg)
		{
			MessagesListBox.Items.Add(msg);
			int lastIndex = MessagesListBox.Items.Count - 1;
			if (lastIndex >= 0)
			{
				MessagesListBox.TopIndex = lastIndex;
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= MessagesListBox.Items.Count)
				return;

			MessagesListBox.Items.RemoveAt(index);

			int lastIndex = MessagesListBox.Items.Count - 1;
			if (lastIndex >= 0)
			{
				MessagesListBox.TopIndex = lastIndex;
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			MessagesListBox.Items.Clear();
			var cbCache = _trayApplicationContext.GetClipboardCache();
			foreach (var item in cbCache)
			{
				MessageWriteLine(item.Text);
			}
		}

		public void RefreshCacheView()
		{
			MessagesListBox.Items.Clear();
			var cache = _trayApplicationContext.GetClipboardCache();
			foreach (var item in cache)
				MessagesListBox.Items.Add(item.Text);
		}

		public void UpdateCheckUpdateNowBtnText(string newString)
		{
			checkUpdateToolStripMenuItem.Text = newString;
		}

		private async void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkUpdateToolStripMenuItem.Enabled = false;
			string currentVersion = Application.ProductVersion;
			var update = await UpdateService.CheckForUpdateAsync(currentVersion, false);
			if (update != null)
			{
				var result = MessageBox.Show(
					"A new version is available. Do you want to download it now? New update features: " + update.Notes,
					"Update Available v" + update.Version,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Information);
				if (result == DialogResult.Yes)
				{
					await UpdateService.DownloadAndRunUpdateAsync();
				}
			}
			else
			{
				MessageBox.Show("No updates available.", "Up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			checkUpdateToolStripMenuItem.Enabled = true;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{

			string aboutHtml = $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <title>About - HelloClipboard</title>
  <style>
    body {{ font-family: Segoe UI, Tahoma, Arial; padding: 16px; color:#222; }}
    h1 {{ margin-top:0; }}
    a {{ color:#1a73e8; text-decoration:none; }}
    a:hover {{ text-decoration:underline; }}
    .meta {{ margin-top:12px; color:#555; }}
    .footer {{ margin-top:20px; font-size:90%; color:#666; }}
  </style>
</head>
<body>
  <h1>HelloClipboard</h1>
  <div class='meta'>Version: {Application.ProductVersion}</div>
  <p>A modern, lightweight clipboard sharing tool for Windows.</p>
  <p>Synchronize your clipboard securely across platforms with ease.</p>
  <p>Developed by <strong>Ali SARIASLAN</strong></p>
  <p>Contact: <a href='mailto:dev@alisariaslan.com'>dev@alisariaslan.com</a></p>
  <div class='footer'>
    GitHub: <a href='https://github.com/alisariaslan/HelloClipboard' target='_blank'>https://github.com/alisariaslan/HelloClipboard</a>
  </div>
</body>
</html>";

			using (var dlg = new InfoDialog("About - HelloClipboard", aboutHtml))
			{
				dlg.ShowDialog(this);
			}
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string helpHtml = @"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <title>Help - HelloClipboard</title>
  <style>
    body { font-family: Segoe UI, Tahoma, Arial; padding:16px; color:#222; }
    h1 { margin-top:0; }
    ul { line-height:1.6; }
    a { color:#1a73e8; text-decoration:none; }
    .note { margin-top:12px; color:#555; }
  </style>
</head>
<body>
  <h1>Help - HelloClipboard</h1>
  <ul>
    <li><strong>Copy</strong> — Copies the selected text to the system clipboard.</li>
    <li><strong>Settings</strong> — Configure basic application options such as starting with Windows and clipboard behavior.</li>
   
  </ul>
  <p class='note'>If you experience any issues, please report them on the GitHub repository or contact <a href='mailto:dev@alisariaslan.com'>dev@alisariaslan.com</a>.</p>
  <p>GitHub: <a href='https://github.com/alisariaslan/HelloClipboard' target='_blank'>https://github.com/alisariaslan/HelloClipboard</a></p>
</body>
</html>";

			using (var dlg = new InfoDialog("Help - HelloClipboard", helpHtml))
			{
				dlg.ShowDialog(this);
			}
		}

		private void ShowUnderDevelopmentDialog(string featureName)
		{
			string html = $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'/>
  <title>{featureName}</title>
  <style>
    body {{ font-family: Segoe UI, Tahoma, Arial; padding: 16px; color:#222; }}
    h1 {{ margin-top:0; font-size: 20px; }}
    p {{ margin-top:12px; color:#555; }}
    a {{ color:#1a73e8; text-decoration:none; }}
    a:hover {{ text-decoration:underline; }}
    .footer {{ margin-top:20px; font-size:90%; color:#666; }}
  </style>
</head>
<body>
  <h1>{featureName}</h1>
  <p>This feature is currently under development and will be available in a future update.</p>
  <div class='footer'>
    Contact: <a href='mailto:dev@alisariaslan.com'>dev@alisariaslan.com</a><br/><br/>
    GitHub: <a href='https://github.com/alisariaslan/HelloClipboard' target='_blank'>https://github.com/alisariaslan/HelloClipboard</a>
  </div>
</body>
</html>";

			using (var dlg = new InfoDialog(featureName, html))
			{
				dlg.ShowDialog(this);
			}
		}


		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (_trayApplicationContext.ApplicationExiting)
			{
				base.OnFormClosing(e);
				return;
			}
			if (!SettingsLoader.Current.HideToTray || TempConfigLoader.Current.AdminPriviligesRequested)
			{
				_trayApplicationContext.ExitApplication();
			}
			else
			{
				e.Cancel = true;
				_trayApplicationContext.HideMainWindow();
			}
		}

		public void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_copyToolStripMenuItem_Click(sender,  e);
		}

		private void _copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessagesListBox.SelectedIndices.Count == 0)
				return;


			CloseDetailFormIfAvaible();

			var cache = _trayApplicationContext.GetClipboardCache();

			List<string> selectedFromCache = new List<string>();

			foreach (int index in MessagesListBox.SelectedIndices)
			{
				if (index >= 0 && index < cache.Count)
					selectedFromCache.Add(cache[index].Text); // artık Text kullanıyoruz
			}

			_trayApplicationContext.SuppressClipboardEvents(true);

			Clipboard.SetText(string.Join(Environment.NewLine, selectedFromCache));

			Task.Delay(100).ContinueWith(_ =>
			{
				_trayApplicationContext.SuppressClipboardEvents(false);
			});
		}



		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			var pos = MessagesListBox.PointToClient(Cursor.Position);

			int index = MessagesListBox.IndexFromPoint(pos);

			if (index < 0)
			{
				e.Cancel = true;
				return;
			}
				MessagesListBox.SelectedIndex = index;

			var cache = _trayApplicationContext.GetClipboardCache();
			if (index >= cache.Count) return;


			CloseDetailFormIfAvaible();

			_openDetailForm = new ClipDetail(this,cache[index]);
			PositionDetailForm(_openDetailForm);
			_openDetailForm.Show(this);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			var cfg = TempConfigLoader.Current;

			if (cfg.MainFormWidth > 0 && cfg.MainFormHeight > 0)
				this.Size = new Size(cfg.MainFormWidth, cfg.MainFormHeight);

			if (cfg.MainFormX >= 0 && cfg.MainFormY >= 0)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = new Point(cfg.MainFormX, cfg.MainFormY);
			}

			_isLoaded = true;
		}

		private void _SaveFormPosition()
		{
			if (this.WindowState != FormWindowState.Minimized && _isLoaded)
			{
				var cfg = TempConfigLoader.Current;
				cfg.MainFormWidth = this.Width;
				cfg.MainFormHeight = this.Height;
				cfg.MainFormX = this.Location.X;
				cfg.MainFormY = this.Location.Y;
				TempConfigLoader.Save();
			}
		}

		public void CloseDetailFormIfAvaible()
		{
			if (_openDetailForm != null && !_openDetailForm.IsDisposed)
			{
				_openDetailForm.Close();
				_openDetailForm = null;
			}

		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			CloseDetailFormIfAvaible();

			_SaveFormPosition();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			_SaveFormPosition();
		}

		private void MainForm_Move(object sender, EventArgs e)
		{
			var screen = Screen.FromControl(this).WorkingArea;
			int snapDistance = 20; 
			int newX = this.Left;
			int newY = this.Top;

			bool snappedX = false;
			bool snappedY = false;

			if (Math.Abs(this.Right - screen.Right) < snapDistance)
			{
				newX = screen.Right - this.Width;
				snappedX = true;
			}
			else if (Math.Abs(this.Left - screen.Left) < snapDistance)
			{
				newX = screen.Left;
				snappedX = true;
			}

			if (Math.Abs(this.Bottom - screen.Bottom) < snapDistance)
			{
				newY = screen.Bottom - this.Height;
				snappedY = true;
			}
			else if (Math.Abs(this.Top - screen.Top) < snapDistance)
			{
				newY = screen.Top;
				snappedY = true;
			}

			if (snappedX || snappedY)
			{
				this.Location = new Point(newX, newY);
			}
		}

		public void ResetFormPositionAndSize()
		{
			Size = new Size(480, 720);
			StartPosition = FormStartPosition.CenterScreen;
			WindowState = FormWindowState.Normal;
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var f = new SettingsForm())
			{
				f.StartPosition = FormStartPosition.CenterParent;
				f.ShowDialog(this);
			}
		}


		private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			int index = MessagesListBox.IndexFromPoint(e.Location);
			if (index < 0) return;

			MessagesListBox.SelectedIndex = index;

			var cache = _trayApplicationContext.GetClipboardCache();
			if (index >= cache.Count) return;


			CloseDetailFormIfAvaible();

			_openDetailForm = new ClipDetail(this,cache[index]);
			PositionDetailForm(_openDetailForm);
			_openDetailForm.Show(this);
		}

		private void PositionDetailForm(Form detailForm)
		{
			var mainRect = this.Bounds;
			var screen = Screen.FromControl(this).WorkingArea;

			int padding = 10;
			Point location = new Point();

			if (mainRect.Right + detailForm.Width + padding <= screen.Right)
			{
				location = new Point(mainRect.Right + padding, mainRect.Top);
			}
			else if (mainRect.Left - detailForm.Width - padding >= screen.Left)
			{
				location = new Point(mainRect.Left - detailForm.Width - padding, mainRect.Top);
			}
			else if (mainRect.Bottom + detailForm.Height + padding <= screen.Bottom)
			{
				location = new Point(mainRect.Left, mainRect.Bottom + padding);
			}
			else if (mainRect.Top - detailForm.Height - padding >= screen.Top)
			{
				location = new Point(mainRect.Left, mainRect.Top - detailForm.Height - padding);
			}
			else
			{
				location = new Point(
					Math.Max(screen.Left, screen.Left + (screen.Width - detailForm.Width) / 2),
					Math.Max(screen.Top, screen.Top + (screen.Height - detailForm.Height) / 2)
				);
			}

			detailForm.StartPosition = FormStartPosition.Manual;
			detailForm.Location = location;
		}

		private void androidSyncToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowUnderDevelopmentDialog("Phone Sync");
		}

		public void FocusSearchBox()
		{
			textBox1_search.Focus();
		}

		public void ClearSearchBox()
		{
			textBox1_search.Text = string.Empty;
		}

		private void textBox1_search_TextChanged(object sender, EventArgs e)
		{
			CloseDetailFormIfAvaible();
			MessagesListBox.ClearSelected();
			FilterClipboardCache(textBox1_search.Text);
		}

		private void FilterClipboardCache(string searchTerm)
		{
			MessagesListBox.BeginUpdate();
			MessagesListBox.Items.Clear();

			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				foreach (var item in _trayApplicationContext.GetClipboardCache())
				{
					MessagesListBox.Items.Add(item.Title);
				}
			}
			else
			{
				var lowerSearch = searchTerm.ToLowerInvariant();
				foreach (var item in _trayApplicationContext.GetClipboardCache())
				{
					if (item.Text != null && item.Text.ToLowerInvariant().Contains(lowerSearch))
					{
						MessagesListBox.Items.Add(item.Title);
					}
				}
			}

			MessagesListBox.EndUpdate();
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			ClearSearchBox();
		}

		private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowUnderDevelopmentDialog("Save to File");
		}
	}
}
