using HelloClipboard.Html;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class MainForm : Form
	{
		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainFormViewModel _viewModel;
		private bool _isLoaded = false;
		private Form _openDetailForm;
		private FormWindowState _previousWindowState = FormWindowState.Normal;

		public MainForm(TrayApplicationContext trayApplicationContext)
		{
			InitializeComponent();

			this.Text = Application.ProductName + " v" + Application.ProductVersion;

			_trayApplicationContext = trayApplicationContext;
			_viewModel = new MainFormViewModel(this, trayApplicationContext);

			_viewModel.LoadSettings();

			MessagesListBox.DisplayMember = "Title";

			var enableClipboardHistory = SettingsLoader.Current.EnableClipboardHistory;

			this.Deactivate += MainForm_Deactivate;
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (!SettingsLoader.Current.AutoHideWhenUnfocus || !_isLoaded)
				return;

			BeginInvoke(new MethodInvoker(() =>
			{
				Form activeForm = Form.ActiveForm;

				if (activeForm == null || (activeForm != this && !IsFormOwnedByMe(activeForm)))
				{
					_trayApplicationContext.HideMainWindow();
				}
			}));
		}

		private bool IsFormOwnedByMe(Form activeForm)
		{
			if (activeForm == null) return false;

			return activeForm == this ||
				   activeForm.Owner == this ||
				   _openDetailForm != null && activeForm == _openDetailForm;
		}

		public void MessageAdd(ClipboardItem item)
		{
			if (SettingsLoader.Current.InvertClipboardHistoryListing)
			{
				MessagesListBox.Items.Insert(0, item);
				MessagesListBox.TopIndex = 0;
			}
			else
			{
				MessagesListBox.Items.Add(item);
				int lastIndex = MessagesListBox.Items.Count - 1;
				if (lastIndex >= 0)
				{
					MessagesListBox.TopIndex = lastIndex;
				}
			}
		}

		public void RemoveOldestMessage()
		{
			if (MessagesListBox.Items.Count == 0)
				return;

			int removeIndex;

			if (SettingsLoader.Current.InvertClipboardHistoryListing)
			{
				removeIndex = MessagesListBox.Items.Count - 1;
			}
			else
			{
				removeIndex = 0;
			}

			MessagesListBox.Items.RemoveAt(removeIndex);

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
			RefreshCacheView();
		}

		public void RefreshCacheView()
		{
			MessagesListBox.BeginUpdate();
			MessagesListBox.Items.Clear();
			var cache = _trayApplicationContext.GetClipboardCache();
			foreach (var item in cache)
			{
				// MessageAdd, item'ın tarih sırasına göre (eskiden yeniye) 
				// gelmesini bekler ve ayar aktifse yeni geleni hep 0. indexe koyar.
				MessageAdd(item);
			}
			MessagesListBox.EndUpdate();
		}

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

		private void OpenDetailForIndex(int index)
		{
			if (index < 0) return;

			MessagesListBox.SelectedIndex = index;

			if (!(MessagesListBox.SelectedItem is ClipboardItem selectedItem))
				return;

			CloseDetailFormIfAvaible();

			if (selectedItem.ItemType == ClipboardItemType.Image)
				_openDetailForm = new ClipDetailImage(this, selectedItem);
			else
			{
				_openDetailForm = new ClipDetailText(this, selectedItem);
			}
			PositionDetailForm(_openDetailForm);
			_openDetailForm.Show(this);
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
			OpenDetailForIndex(index);
		}
		private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			int index = MessagesListBox.IndexFromPoint(e.Location);
			OpenDetailForIndex(index);
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

			if (SettingsLoader.Current.AlwaysTopMost)
			{
				this.TopMost = true;
			}
			CheckAndUpdateTopMostImage();

			this.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;

			_isLoaded = true;
		}

		public void UpdateTaskbarVisibility(bool visible)
		{
			this.ShowInTaskbar = visible;
		}

		private void _SaveFormPosition()
		{
			if (this.WindowState == FormWindowState.Normal && _isLoaded)
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

		private async void MainForm_Resize(object sender, EventArgs e)
		{
			_SaveFormPosition();
			if (_isLoaded && this.WindowState == FormWindowState.Normal &&
		_previousWindowState == FormWindowState.Maximized)
			{
				await System.Threading.Tasks.Task.Delay(10);
				var cfg = TempConfigLoader.Current;
				if (cfg.MainFormX >= 0 && cfg.MainFormY >= 0)
				{
					this.Location = new Point(cfg.MainFormX, cfg.MainFormY);
				}
			}
			_previousWindowState = this.WindowState;
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
			if (_openDetailForm != null && !_openDetailForm.IsDisposed)
			{
				PositionDetailForm(_openDetailForm);
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
			using (var f = new SettingsForm(this))
			{
				f.StartPosition = FormStartPosition.CenterParent;
				f.ShowDialog(this);
			}
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

			var cache = _trayApplicationContext.GetClipboardCache();
			var itemsToDisplay = string.IsNullOrWhiteSpace(searchTerm)
				? cache
				: cache.Where(i => i.Content != null && i.Content.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()));

			foreach (var item in itemsToDisplay)
			{
				// Direkt Add(item) yaparsanız ayarı görmezden gelip sona ekler.
				// MessageAdd(item) metodunu kullanırsanız ayara göre başa veya sona ekler.
				MessageAdd(item);
			}

			MessagesListBox.EndUpdate();
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show(
				"Are you sure you want to clear the clipboard history?",
				"Clear Clipboard",
				MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes)
				_trayApplicationContext.ClearClipboard();
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

		public void CheckAndUpdateTopMostImage()
		{
			if (this.TopMost)
				pictureBox3_topMost.Image = Properties.Resources.icons8_locked_192px;
			else
				pictureBox3_topMost.Image = Properties.Resources.icons8_unlocked_192px;
		}

		private void pictureBox3_topMost_Click(object sender, EventArgs e)
		{
			if (!this.TopMost)
			{
				this.TopMost = true;
			}
			else
			{
				if (SettingsLoader.Current.AlwaysTopMost)
				{
					MessageBox.Show("The 'Always Top Most' setting is enabled in settings. Please disable it there to turn off top-most behavior.", "Action Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				this.TopMost = false;
			}
			CheckAndUpdateTopMostImage();
		}
	}
}
