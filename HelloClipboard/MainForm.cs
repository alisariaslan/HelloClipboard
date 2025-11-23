using HelloClipboard.Html;
using System;
using System.ComponentModel;
using System.Drawing;
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
			_viewModel = new MainFormViewModel(this,trayApplicationContext);

			_viewModel.LoadSettings();

			MessagesListBox.DisplayMember = "Title";
		}

		public void MessageAdd(ClipboardItem item)
		{
			MessagesListBox.Items.Add(item);
			int lastIndex = MessagesListBox.Items.Count - 1;
			if (lastIndex >= 0)
			{
				MessagesListBox.TopIndex = lastIndex;
			}
		}

		public void MessageRemoveAt(int index)
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

		public void MessageRemoveItem(ClipboardItem item)
		{
			if (MessagesListBox.Items.Contains(item))
			{
				MessagesListBox.Items.Remove(item);
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			MessagesListBox.Items.Clear();
			var cbCache = _trayApplicationContext.GetClipboardCache();
			foreach (var item in cbCache)
			{
				MessageAdd(item);
			}
		}

		//public void RefreshCacheView()
		//{
		//	MessagesListBox.Items.Clear();
		//	var cache = _trayApplicationContext.GetClipboardCache();
		//	foreach (var item in cache)
		//		MessagesListBox.Items.Add(item.Content);
		//}


		public void UpdateCheckUpdateNowBtnText(string newString)
		{
			checkUpdateToolStripMenuItem.Text = newString;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var dlg = new InfoDialog(AboutHtml.GetTitle(), AboutHtml.GetHtml()))
			{
				dlg.ShowDialog(this);
			}
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var dlg = new InfoDialog(HelpHtml.GetTitle(), HelpHtml.GetHtml()))
			{
				dlg.ShowDialog(this);
			}
		}

		private void ShowUnderDevelopmentDialog(string featureName)
		{
			using (var dlg = new InfoDialog(UnderDevelopmentHtml.GetTitle(), UnderDevelopmentHtml.GetHtml(featureName)))
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
			if (MessagesListBox.SelectedIndices.Count == 0)
				return;
			CloseDetailFormIfAvaible();
			ClipboardItem selectedItem = MessagesListBox.SelectedItem as ClipboardItem;
			_viewModel.CopyClicked(selectedItem);
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
			ClipboardItem selectedItem = MessagesListBox.SelectedItem as ClipboardItem;
			if (selectedItem == null)
			{
				e.Cancel = true;
				return;
			}
			CloseDetailFormIfAvaible();
			_openDetailForm = new ClipDetail(this, selectedItem);
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
			ClipboardItem selectedItem = MessagesListBox.SelectedItem as ClipboardItem;
			if (selectedItem == null) return; 
			CloseDetailFormIfAvaible();
			_openDetailForm = new ClipDetail(this, selectedItem);
			PositionDetailForm(_openDetailForm);
			_openDetailForm.Show(this);
		}

		private void PositionDetailForm(Form detailForm)
		{
			var mainRect = this.Bounds;
			var screen = Screen.FromControl(this).WorkingArea;
			int padding = 1;
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

		private void phoneSyncToolStripMenuItem_Click(object sender, EventArgs e)
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
					MessagesListBox.Items.Add(item);
				}
			}
			else
			{
				var lowerSearch = searchTerm.ToLowerInvariant();
				foreach (var item in _trayApplicationContext.GetClipboardCache())
				{
					if (item.Content != null && item.Content.ToLowerInvariant().Contains(lowerSearch))
					{
						MessagesListBox.Items.Add(item);
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
	}
}
