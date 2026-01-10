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
		#region FIELDS & CONSTRUCTOR

		private readonly TrayApplicationContext _trayApplicationContext;
		private readonly MainFormViewModel _viewModel;
		private DetailWindowManager _detailManager;
		private FormLayoutManager _layoutManager;
		private string _currentSearchTerm = string.Empty;

		public MainForm(TrayApplicationContext trayApplicationContext)
		{
			InitializeComponent();

			this.Text = Application.ProductName + " v" + Application.ProductVersion;
			_trayApplicationContext = trayApplicationContext;
			_viewModel = new MainFormViewModel(trayApplicationContext);

			// Form Events
			this.Load += MainForm_Load;
			this.Shown += MainForm_Shown;
			this.FormClosing += MainForm_FormClosing;
			this.Resize += MainForm_Resize;
			this.Move += MainForm_Move;
			this.Deactivate += (s, e) => MainFormDeactivated();

			// ListBox Events
			MessagesListBox.DisplayMember = "Title";
			MessagesListBox.DrawMode = DrawMode.OwnerDrawFixed;
			MessagesListBox.DrawItem += MessagesListBox_DrawItem;
			MessagesListBox.Resize += (s, e) => MessagesListBox.Invalidate();
			MessagesListBox.SelectedIndexChanged += MessagesListBox_SelectedIndexChanged;
			MessagesListBox.MouseClick += MessagesListBox_MouseClick;

			// Search Box Events
			textBox1_search.KeyDown += textBox1_search_KeyDown;
			textBox1_search.KeyPress += textBox1_search_KeyPress;
			textBox1_search.TextChanged += textBox1_search_TextChanged;

			// Other things
			_detailManager = new DetailWindowManager(this, (s, e) => MainFormDeactivated());
			_layoutManager = new FormLayoutManager(this, _detailManager);
		}

		#endregion

		#region FORM LIFECYCLE

		private void MainForm_Load(object sender, EventArgs e)
		{
			_layoutManager.OnLoad();
			CheckAndUpdateTopMostImage();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			_layoutManager.OnShown();
			UpdatePrivacyStatusUI();
			RefreshCacheView();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			_layoutManager?.OnResize();
		}

		private void MainForm_Move(object sender, EventArgs e)
		{
			_layoutManager?.OnMove();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_layoutManager.OnClosing();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (_trayApplicationContext.ApplicationExiting) return;

			if (!SettingsLoader.Current.HideToTray || TempConfigLoader.Current.AdminPriviligesRequested)
			{
				_trayApplicationContext.ExitApplication();
			}
			else
			{
				e.Cancel = true;
				_layoutManager.OnClosing();
				_trayApplicationContext.HideMainWindow();
			}
		}


		private bool _isLocked = false;
		private void MainFormDeactivated()
		{
			if (_isLocked)
				return;

			if (!SettingsLoader.Current.AutoHideWhenUnfocus || !_layoutManager.IsLoaded)
				return;

			BeginInvoke(new MethodInvoker(async () =>
			{
				await System.Threading.Tasks.Task.Delay(150); // Biraz daha toleranslı bir süre
				if (!NativeMethods.IsCurrentProcessFocused())
				{
					_trayApplicationContext.HideMainWindow();
				}
			}));
		}

		#endregion

		#region LISTBOX OPERATIONS

		public void MessageAdd(ClipboardItem item)
		{
			int index = _viewModel.GetInsertionIndex(
				item,
				MessagesListBox.Items.Count,
				(i) => MessagesListBox.Items[i] as ClipboardItem
			);

			MessagesListBox.Items.Insert(index, item);

			if (SettingsLoader.Current.InvertClipboardHistoryListing)
			{
				MessagesListBox.TopIndex = 0;
			}
			else
			{
				MessagesListBox.TopIndex = MessagesListBox.Items.Count - 1;
			}
		}

		public void RemoveOldestMessage()
		{
			int removeIndex = _viewModel.GetIndexToRemove(MessagesListBox.Items.Cast<ClipboardItem>());
			if (removeIndex != -1)
			{
				MessagesListBox.Items.RemoveAt(removeIndex);
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
			if (MessagesListBox.InvokeRequired)
			{
				MessagesListBox.Invoke(new Action(RefreshCacheView));
				return;
			}

			MessagesListBox.BeginUpdate();
			try
			{
				MessagesListBox.Items.Clear();
				var displayList = _viewModel.GetDisplayList(_currentSearchTerm);
				foreach (var item in displayList)
				{
					MessagesListBox.Items.Add(item);
				}
			}
			finally
			{
				MessagesListBox.EndUpdate();
				UpdateStatusLabel();
			}

			if (MessagesListBox.Items.Count > 0)
			{
				MessagesListBox.TopIndex = SettingsLoader.Current.InvertClipboardHistoryListing
					? 0
					: MessagesListBox.Items.Count - 1;
			}

			MessagesListBox.Refresh();
		}

		private void MessagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MessagesListBox.SelectedIndex >= 0)
			{
				OpenDetailForIndex(MessagesListBox.SelectedIndex);

				// --- GÜNCELLEME ---
				if (!SettingsLoader.Current.FocusDetailWindow)
				{
					textBox1_search.Focus();
				}

				UpdateMenuStates();
			}
		}

		private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;
			int index = MessagesListBox.IndexFromPoint(e.Location);
			OpenDetailForIndex(index);

			// --- GÜNCELLEME ---
			if (!SettingsLoader.Current.FocusDetailWindow)
			{
				textBox1_search.Focus();
			}
		}

		private void MessagesListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			DrawingHelper.RenderClipboardItem(e, MessagesListBox, _currentSearchTerm, _viewModel);
		}

		private void OpenDetailForIndex(int index)
		{
			if (index < 0) return;
			MessagesListBox.SelectedIndex = index;
			if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
			{
				Rectangle currentBounds = _detailManager.GetActiveForm()?.Bounds ?? Rectangle.Empty;
				_detailManager.ShowDetail(selectedItem, currentBounds);
			}
		}

	

		#endregion

		#region SEARCH OPERATIONS

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

		private void textBox1_search_KeyDown(object sender, KeyEventArgs e)
		{
			if (textBox1_search.HandleWordDeletion(e))
			{
				e.SuppressKeyPress = true;
				return;
			}

			if (MessagesListBox.Items.Count > 0)
			{
				switch (e.KeyCode)
				{
					case Keys.Down:
						MessagesListBox.MoveSelection(1);
						e.Handled = e.SuppressKeyPress = true;
						break;
					case Keys.Up:
						MessagesListBox.MoveSelection(-1);
						e.Handled = e.SuppressKeyPress = true;
						break;
					case Keys.Enter:
						if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
						{
							_viewModel.CopyClicked(selectedItem, asObject: false);
							_trayApplicationContext.HideMainWindow();
						}
						e.Handled = e.SuppressKeyPress = true;
						break;
				}
			}
		}

		private void textBox1_search_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsControl(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void checkBoxRegex_CheckedChanged(object sender, EventArgs e)
		{
			_viewModel.SetSearchOptions(checkBoxRegex.Checked, checkBoxCaseSensitive.Checked);
			textBox1_search_TextChanged(null, null);
		}

		private void checkBoxCaseSensitive_CheckedChanged(object sender, EventArgs e)
		{
			_viewModel.SetSearchOptions(checkBoxRegex.Checked, checkBoxCaseSensitive.Checked);
			RefreshFilteredList();
		}

		public void FocusSearchBox() => textBox1_search.Focus();
		public void ClearSearchBox() => textBox1_search.Text = string.Empty;

		#endregion

		#region CONTEXT MENU & ITEM ACTIONS

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			var pos = MessagesListBox.PointToClient(Cursor.Position);
			int index = MessagesListBox.IndexFromPoint(pos);
			if (index < 0) { e.Cancel = true; return; }
			OpenDetailForIndex(index);
			UpdateMenuStates();
		}

		public void CopyCliked( bool asObject = false)
		{
			if (MessagesListBox.SelectedIndices.Count == 0) return;
			CloseDetailFormIfAvaible();
			ClipboardItem selectedItem = MessagesListBox.SelectedItem as ClipboardItem;
			_viewModel.CopyClicked(selectedItem,asObject);
		}

		public void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyCliked();
		}

		private void pinUnpinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessagesListBox.SelectedItem is ClipboardItem selected)
			{
			
				bool isPinned = _viewModel.TogglePin(selected);

			
				MessagesListBox.BeginUpdate();

		
				MessagesListBox.Items.Remove(selected);

				int targetVisualIndex = _viewModel.GetVisualInsertionIndex(selected, MessagesListBox.Items);

				MessagesListBox.Items.Insert(targetVisualIndex, selected);
				MessagesListBox.SelectedIndex = targetVisualIndex;

				MessagesListBox.EndUpdate();

				pinUnpinToolStripMenuItem.Checked = isPinned;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;
			var saveInfo = _viewModel.GetSaveFileInfo(selected);
			using (var dialog = new SaveFileDialog())
			{
				dialog.Title = saveInfo.Title;
				dialog.Filter = saveInfo.Filter;
				dialog.FileName = saveInfo.FileName;
				if (dialog.ShowDialog(this) == DialogResult.OK)
					_viewModel.SaveItemToDisk(selected, dialog.FileName);
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessagesListBox.SelectedItem is ClipboardItem selected)
			{
				FileOpener.OpenItem(selected);
			}
		}

		private void UpdateMenuStates()
		{
			if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;

			// --- 1. PIN DURUMU ---
			pinUnpinToolStripMenuItem.Checked = selected.IsPinned;
			pinUnpinToolStripMenuItem.Text = selected.IsPinned ? "Unpin" : "Pin";

			// --- 2. KOŞUL HESAPLAMALARI ---
			bool isUrl = selected.ItemType == ClipboardItemType.Text && UrlHelper.IsValidUrl(selected.Content);

			// Path geçerliliği (Hem dosya hem klasör kontrolü)
			bool isFileExists = File.Exists(selected.Content);
			bool isDirExists = !string.IsNullOrEmpty(selected.Content) && Directory.Exists(selected.Content);
			bool isPathFound = isFileExists || isDirExists;

			bool isImage = selected.ItemType == ClipboardItemType.Image && selected.ImageContent != null;
			bool isText = selected.ItemType == ClipboardItemType.Text && !string.IsNullOrEmpty(selected.Content);

			// --- 3. COPY (KOPYALA) KONTROLÜ ---
			// Eğer bir yol (Path) öğesiyse sadece diskte varsa aktif olsun. 
			// Metin ve Resim öğeleri her zaman kopyalanabilir.
			if (selected.ItemType == ClipboardItemType.Path || selected.ItemType == ClipboardItemType.Path)
			{
				copyToolStripMenuItem.Enabled = isPathFound;
			}
			else
			{
				copyToolStripMenuItem.Enabled = true;
			}

			// --- 4. OPEN (AÇ) KONTROLÜ ---
			openToolStripMenuItem.Visible = true;
			openToolStripMenuItem.Enabled = isUrl || isPathFound || isImage || isText;

			// --- 5. SAVE (KAYDET) KONTROLÜ ---
			// Eğer Path tipindeyse ve dosya bulunamadıysa (veya klasörse) Save disabled olsun.
			var canSave = selected.ItemType != ClipboardItemType.Path || isFileExists;
			saveToolStripMenuItem.Enabled = canSave;
		}

		private void pcbox_clearClipboard_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to clear the clipboard history?", "Clear Clipboard", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes) _trayApplicationContext.ClearClipboard();
		}

		#endregion

		#region NAVIGATION & DIALOGS

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var f = new SettingsForm(this))
			{
				f.StartPosition = FormStartPosition.CenterParent;
				f.ShowDialog(this);
			}
		}

		private async void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkUpdateToolStripMenuItem.Enabled = false;
			var updateService = _trayApplicationContext.GetUpdateService();
			var update = await updateService.CheckForUpdateAsync(Application.ProductVersion, false);

			if (update != null)
			{
				var result = MessageBox.Show($"New version v{update.Version} is available!\n\nNotes: {update.Notes}\n\nDownload now?", "Update Found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (result == DialogResult.Yes) await updateService.DownloadAndRunUpdateAsync();
			}
			else MessageBox.Show("Your application is up to date.", "No Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

			checkUpdateToolStripMenuItem.Enabled = true;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => ShowHtmlDialog(AboutHtml.GetTitle(), AboutHtml.GetHtml());
		private void helpToolStripMenuItem_Click(object sender, EventArgs e) => ShowHtmlDialog(HelpHtml.GetTitle(), HelpHtml.GetHtml());
		private void phoneSyncToolStripMenuItem_Click(object sender, EventArgs e) => ShowUnderDevelopmentDialog("Phone Sync");

		private void ShowUnderDevelopmentDialog(string featureName) => ShowHtmlDialog(UnderDevelopmentHtml.GetTitle(), UnderDevelopmentHtml.GetHtml(featureName));
		private void ShowHtmlDialog(string title, string html)
		{
			using (var dlg = new InfoDialog(title, html)) dlg.ShowDialog(this);
		}

		#endregion

		#region UI STATE HELPERS

		public void UpdateCheckUpdateNowBtnText(string newString) => checkUpdateToolStripMenuItem.Text = newString;
		public void UpdateTaskbarVisibility(bool visible) => this.ShowInTaskbar = visible;
		public void CloseDetailFormIfAvaible() => _detailManager.CloseAll();
		public void ResetFormPositionAndSize() => _layoutManager.ResetToDefault();

		public void CheckAndUpdateTopMostImage()
		{
			// Resim kaynaklarını projenizdeki ikon isimlerine göre güncelleyin
			pcbox_topMost.Image = _isLocked
		? Properties.Resources.icons8_locked_192px
		: Properties.Resources.icons8_unlocked_192px;

			// Görsel bir geri bildirim için (opsiyonel) Arka plan rengi değişimi
			pcbox_topMost.BackColor = _isLocked ? Color.LightBlue : Color.Transparent;
		}

		public void UpdatePrivacyStatusUI()
		{
			bool isActive = _trayApplicationContext.IsPrivacyModeActive;
			// Gizlilik modu için uygun ikonları seçin (Örn: Eye / Eye-off)
			pcbox_togglePrivacy.Image = isActive ? Properties.Resources.hide_50px : Properties.Resources.eye_50px;

			// Liste kutusunu gizlilik moduna göre tazele (İçerikler maskelensin diye)
			RefreshCacheView();

		}
		public void UpdateStatusLabel()
		{
			if (toolStripStatusLabel1 == null) return;

			int memoryCount = _trayApplicationContext.GetClipboardCache().Count;
			int storedCount = _trayApplicationContext.GetStoredCount();

			// İngilizce formatta yazdırıyoruz
			toolStripStatusLabel1.Text = $"Memory: {memoryCount} | Stored: {storedCount}";

		}
		private void pcbox_topMost_Click(object sender, EventArgs e)
		{
			// 1. Durumu tersine çevir
			_isLocked = !_isLocked;

			// 2. Formun TopMost özelliğini kilit durumuna göre ayarla
			// (Ancak ayarlardan "AlwaysTopMost" açıksa her zaman true kalmalı)
			this.TopMost = _isLocked || SettingsLoader.Current.AlwaysTopMost;

			CheckAndUpdateTopMostImage();
		}

		#endregion

		private void cancelStripMenuItem1_Click(object sender, EventArgs e)
		{
			contextMenuStrip1?.Close();
		}

		// MainForm.cs
		private void delToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;

			// 1. Görsel hazırlık
			CloseDetailFormIfAvaible();

			// 2. İş mantığını ViewModel'e devret
			_viewModel.DeleteItem(selected);

			// 3. ListBox'ı güncelle
			MessagesListBox.Items.Remove(selected);
			UpdateStatusLabel();

			textBox1_search.Focus();
		}

		private void pcbox_togglePrivacy_Click(object sender, EventArgs e)
		{
			_trayApplicationContext.TogglePrivacyMode();
			UpdatePrivacyStatusUI();
		}
	}
}