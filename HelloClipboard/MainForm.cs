using HelloClipboard.Html;
using HelloClipboard.Services;
using HelloClipboard.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HelloClipboard
{
	public partial class MainForm : Form
	{
		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainFormViewModel _viewModel;
		private bool _isLoaded = false;
		private FormWindowState _previousWindowState = FormWindowState.Normal;
		private DetailWindowManager _detailManager;
		private string _currentSearchTerm = string.Empty;

		public MainForm(TrayApplicationContext trayApplicationContext)
		{
			InitializeComponent();

			this.Text = Application.ProductName + " v" + Application.ProductVersion;

			_trayApplicationContext = trayApplicationContext;
			_viewModel = new MainFormViewModel(trayApplicationContext);

			_viewModel.LoadSettings();

			MessagesListBox.DisplayMember = "Title";
			MessagesListBox.DrawMode = DrawMode.OwnerDrawFixed;
			MessagesListBox.DrawItem += MessagesListBox_DrawItem;
			MessagesListBox.Resize += (s, e) => MessagesListBox.Invalidate(); // force redraw so ellipsis reflows after resize

			var enableClipboardHistory = SettingsLoader.Current.EnableClipboardHistory;

			this.Deactivate += MainForm_Deactivate;

			MessagesListBox.SelectedIndexChanged += MessagesListBox_SelectedIndexChanged;
			textBox1_search.KeyDown += textBox1_search_KeyDown;
			textBox1_search.KeyPress += textBox1_search_KeyPress;

			_detailManager = new DetailWindowManager(this, MainForm_Deactivate);
		}

		#region FORM EVENTS
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			MainFormDeactivated();
		}

		private void textBox1_search_KeyDown(object sender, KeyEventArgs e)
		{
			TextBoxSearchKeydown(e);
		}
		private void MessagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			MessagesListBoxSelectedIndexChanged();
		}
		private void MainForm_Shown(object sender, EventArgs e)
		{
			RefreshCacheView();
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
		public void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyClicked();
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			ContextMenuStrip1Opening(e);
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
		#endregion

		void TextBoxSearchKeydown(KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.Back)
			{
				textBox1_search.DeletePreviousWord(); 
				e.SuppressKeyPress = true;
				return;
			}
			if (e.Control && e.KeyCode == Keys.Delete)
			{
				textBox1_search.DeleteNextWord(); 
				e.SuppressKeyPress = true;
				return;
			}
			if (MessagesListBox.Items.Count == 0) return;

			int currentIndex = MessagesListBox.SelectedIndex;

			if (e.KeyCode == Keys.Down)
			{
				// Bir alt satıra geç (liste sonundaysa başa dönme veya durma tercihi sana ait)
				if (currentIndex < MessagesListBox.Items.Count - 1)
					MessagesListBox.SelectedIndex = currentIndex + 1;

				e.Handled = true; // Windows'un bip sesini ve imleç hareketini engeller
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Up)
			{
				// Bir üst satıra geç
				if (currentIndex > 0)
					MessagesListBox.SelectedIndex = currentIndex - 1;
				else if (currentIndex == -1 && MessagesListBox.Items.Count > 0)
					MessagesListBox.SelectedIndex = 0;

				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Enter)
			{
				// Enter'a basıldığında seçili öğeyi kopyala ve kapat
				if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
				{
					_viewModel.CopyClicked(selectedItem);
					_trayApplicationContext.HideMainWindow();
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		void MessagesListBoxSelectedIndexChanged()
		{
			if (MessagesListBox.SelectedIndex >= 0)
			{
				// Klavye ile gezerken detay penceresini otomatik aç/güncelle
				OpenDetailForIndex(MessagesListBox.SelectedIndex);

				// Odağı tekrar arama kutusuna çek (Detay formu odağı çalmasın diye)
				textBox1_search.Focus();

				if (MessagesListBox.SelectedItem is ClipboardItem selected)
				{
					pinToolStripMenuItem.Checked = selected.IsPinned;
					pinToolStripMenuItem.Text = selected.IsPinned ? "Unpin" : "Pin";
				}
			}
		}

		void MainFormDeactivated()
		{
			if (!SettingsLoader.Current.AutoHideWhenUnfocus || !_isLoaded)
				return;

			BeginInvoke(new MethodInvoker(async () =>
			{
				await System.Threading.Tasks.Task.Delay(100);

				// NativeMethods içindeki temiz metodu çağırdık
				if (!NativeMethods.IsCurrentProcessFocused())
				{
					_trayApplicationContext.HideMainWindow();
				}
			}));
		}

		public void MessageAdd(ClipboardItem item)
		{
			int index = _viewModel.GetInsertionIndex(
				item,
				MessagesListBox.Items.Count,
				(i) => MessagesListBox.Items[i] as ClipboardItem
			);

			MessagesListBox.Items.Insert(index, item);

			// Otomatik kaydırma mantığı
			if (SettingsLoader.Current.InvertClipboardHistoryListing)
				MessagesListBox.TopIndex = index;
			else
				MessagesListBox.TopIndex = MessagesListBox.Items.Count - 1;
		}

		public void RemoveOldestMessage()
		{
			int removeIndex = _viewModel.GetIndexToRemove(MessagesListBox.Items.Cast<ClipboardItem>());

			if (removeIndex != -1)
			{
				MessagesListBox.Items.RemoveAt(removeIndex);

				// Kaydırma mantığı (UI ile ilgili olduğu için burada kalabilir)
				if (MessagesListBox.Items.Count > 0)
					MessagesListBox.TopIndex = SettingsLoader.Current.InvertClipboardHistoryListing ? 0 : MessagesListBox.Items.Count - 1;
			}
		}

		public void MessageRemoveItem(ClipboardItem item)
		{
			if (MessagesListBox.Items.Contains(item))
			{
				MessagesListBox.Items.Remove(item);
				MessagesListBox.Refresh();
			}
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

		public void CopyClicked()
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

			if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
			{
				// Manager her şeyi hallediyor
				Rectangle currentBounds = _detailManager.GetActiveForm()?.Bounds ?? Rectangle.Empty;
				_detailManager.ShowDetail(selectedItem, currentBounds);
			}
		}


		void ContextMenuStrip1Opening(CancelEventArgs e)
		{
			var pos = MessagesListBox.PointToClient(Cursor.Position);
			int index = MessagesListBox.IndexFromPoint(pos);
			if (index < 0)
			{
				e.Cancel = true;
				return;
			}
			OpenDetailForIndex(index);
			if (MessagesListBox.SelectedItem is ClipboardItem selected)
			{
				pinToolStripMenuItem.Checked = selected.IsPinned;
				pinToolStripMenuItem.Text = selected.IsPinned ? "Unpin" : "Pin";

				var isUrl = selected.ItemType == ClipboardItemType.Text && UrlHelper.IsValidUrl(selected.Content);
				openUrlToolStripMenuItem.Visible = isUrl;

				var fileExists = selected.ItemType != ClipboardItemType.File || (!string.IsNullOrWhiteSpace(selected.Content) && File.Exists(selected.Content));
				saveToFileToolStripMenuItem.Enabled = fileExists;
			}
		}

		private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			int index = MessagesListBox.IndexFromPoint(e.Location);
			OpenDetailForIndex(index);

			// Detay formu otomatik açılırken odağı çalmasın diye arama kutusuna geri ver
			textBox1_search.Focus();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			FormPersistence.ApplyStoredGeometry(this); // Utils kullanıldı

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
			if (_isLoaded)
			{
				FormPersistence.SaveGeometry(this); // Utils kullanıldı
			}
		}

		public void CloseDetailFormIfAvaible()
		{
			_detailManager.CloseAll();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			CloseDetailFormIfAvaible();
			_SaveFormPosition();
		}

		private async void MainForm_Resize(object sender, EventArgs e)
		{
			_SaveFormPosition();
			if (_isLoaded && this.WindowState == FormWindowState.Normal && _previousWindowState == FormWindowState.Maximized)
			{
				await System.Threading.Tasks.Task.Delay(10);
				FormPersistence.ApplyStoredGeometry(this);
			}
			_previousWindowState = this.WindowState;

			// Manager üzerinden kontrol et ve konumlandır
			if (_detailManager.IsAnyVisible())
			{
				_detailManager.PositionFormNextToOwner(_detailManager.GetActiveForm());
			}
		}

		private void MainForm_Move(object sender, EventArgs e)
		{
			this.Location = WindowHelper.GetSnappedLocation(this);

			// Eğer bir detay formu açıksa, onu ana formun yanına hizala
			if (_detailManager.IsAnyVisible())
			{
				_detailManager.PositionFormNextToOwner(_detailManager.GetActiveForm());
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
			_currentSearchTerm = textBox1_search.Text;
			CloseDetailFormIfAvaible();
			MessagesListBox.ClearSelected();
			RefreshFilteredList();
		}

		private void RefreshFilteredList()
		{
			MessagesListBox.BeginUpdate();
			MessagesListBox.Items.Clear();
			foreach (var item in _viewModel.GetFilteredItems(_currentSearchTerm))
			{
				MessageAdd(item);
			}
			MessagesListBox.EndUpdate();
		}

		private void textBox1_search_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Ctrl ile basılan kontrol karakterlerinin kutu olarak görünmesini engelle
			if (char.IsControl(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
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

		private void checkBoxRegex_CheckedChanged(object sender, EventArgs e)
		{
			_viewModel.SetSearchOptions(checkBoxRegex.Checked, checkBoxCaseSensitive.Checked);
			textBox1_search_TextChanged(null, null);
		}

		private void checkBoxCaseSensitive_CheckedChanged(object sender, EventArgs e)
		{
			// ViewModel'e yeni ayarları gönder
			_viewModel.SetSearchOptions(checkBoxRegex.Checked, checkBoxCaseSensitive.Checked);

			// Listeyi tazele
			RefreshFilteredList();
		}

		private void pinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessagesListBox.SelectedItem is ClipboardItem selected)
			{
				bool isPinned = _viewModel.TogglePin(selected);

				MessagesListBox.BeginUpdate();
				MessagesListBox.Items.Remove(selected);
				if (isPinned)
					MessagesListBox.Items.Insert(0, selected);
				else
				{
					int offset = MessagesListBox.Items.Cast<object>().TakeWhile(i => (i as ClipboardItem)?.IsPinned == true).Count();
					MessagesListBox.Items.Insert(offset, selected);
				}
				MessagesListBox.EndUpdate();
				MessagesListBox.Refresh();

				pinToolStripMenuItem.Checked = isPinned;
				pinToolStripMenuItem.Text = isPinned ? "Unpin" : "Pin";
			}
		}

		private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;

			var saveInfo = _viewModel.GetSaveFileInfo(selected);

			using (var dialog = new SaveFileDialog())
			{
				dialog.Title = saveInfo.Title;
				dialog.Filter = saveInfo.Filter;
				dialog.FileName = saveInfo.FileName;

				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					_viewModel.SaveItemToDisk(selected, dialog.FileName);
				}
			}
		}

		private void openUrlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;

			try
			{
				UrlHelper.OpenUrl(selected.Content);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "URL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkUpdateToolStripMenuItem.Enabled = false;

			// Context üzerinden instance servise erişiyoruz
			var updateService = _trayApplicationContext.GetUpdateService();
			var update = await updateService.CheckForUpdateAsync(Application.ProductVersion, false);

			if (update != null)
			{
				var result = MessageBox.Show(
					$"New version v{update.Version} is available!\n\nNotes: {update.Notes}\n\nDownload now?",
					"Update Found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

				if (result == DialogResult.Yes)
				{
					await updateService.DownloadAndRunUpdateAsync();
				}
			}
			else
			{
				MessageBox.Show("Your application is up to date.", "No Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

		private void MessagesListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index < 0 || e.Index >= MessagesListBox.Items.Count)
				return;

			var item = MessagesListBox.Items[e.Index] as ClipboardItem;
			if (item == null)
				return;

			bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			Color textColor = selected ? SystemColors.HighlightText : SystemColors.ControlText;
			var bounds = e.Bounds;

			string displayText = item.Title ?? string.Empty;
			if (item.IsPinned)
				displayText = "[PIN] " + displayText;

			DrawingHelper.DrawTextWithHighlight(e.Graphics, displayText, e.Font, textColor, bounds, selected, _currentSearchTerm, _viewModel.GetHighlightRegex(_currentSearchTerm), _viewModel.CaseSensitive);

			e.DrawFocusRectangle();
		}

	}
}
