using HelloClipboard.Constants;
using HelloClipboard.Html;
using HelloClipboard.Models;
using HelloClipboard.Services;
using HelloClipboard.Utils;
using HelloClipboard.Views;
using ReaLTaiizor.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HelloClipboard
{
    public partial class MainForm : PoisonForm
    {
        #region FIELDS & CONSTRUCTOR
        private readonly TrayApplicationContext _trayApplicationContext;
        private readonly MainFormViewModel _viewModel;
        private DetailWindowManager _detailManager;
        private FormLayoutManager _layoutManager;
        private string _currentSearchTerm = string.Empty;
        private bool _suppressAutoHide = false;
        private bool _isSnippetMode = false;
        public MainFormViewModel ViewModel => _viewModel;
        public MainForm(TrayApplicationContext trayApplicationContext)
        {
            InitializeComponent();
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            this.MessagesListBox.BackColor = this.BackColor;
            this.Text = $"{Application.ProductName}";
            _trayApplicationContext = trayApplicationContext;
            _viewModel = new MainFormViewModel(trayApplicationContext);
            // Form Events
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            this.Resize += MainForm_Resize;
            this.Move += MainForm_Move;
            this.Deactivate += (s, e) => MainFormDeactivated();
            // ListBox Events - DisplayMember is only a fallback; OwnerDraw handles rendering
            MessagesListBox.DisplayMember = "Title";
            MessagesListBox.DrawMode = DrawMode.OwnerDrawFixed;
            MessagesListBox.ItemHeight = 24;
            MessagesListBox.DrawItem += MessagesListBox_DrawItem;
            MessagesListBox.Resize += (s, e) => MessagesListBox.Invalidate();
            MessagesListBox.SelectedIndexChanged += MessagesListBox_SelectedIndexChanged;
            MessagesListBox.MouseClick += MessagesListBox_MouseClick;
            MessagesListBox.MouseWheel += MessagesListBox_MouseWheel;
            MessagesListBox.DoubleClick += MessagesListBox_DoubleClick;
            // Search Box Events
            poisonTextBox1_search.KeyDown += poisonTextBox1_search_KeyDown;
            poisonTextBox1_search.KeyPress += poisonTextBox1_search_KeyPress;
            poisonTextBox1_search.TextChanged += poisonTextBox1_search_TextChanged;
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
            RefreshTagFilter(); // Kaydedilen tag'leri butona yükle
            RefreshList();
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            _layoutManager?.OnResize();
        }
        private void MainForm_Move(object sender, EventArgs e)
        {
            _layoutManager?.OnMove();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _layoutManager.OnClosing();
            if (_trayApplicationContext.ApplicationExiting)
            {
                return;
            }
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
        private void MainFormDeactivated()
        {
            if (_viewModel.IsLocked || _suppressAutoHide)
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
        private void MessagesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (_isSnippetMode && MessagesListBox.SelectedItem is SnippetItem snippet)
            {
                PasteSnippet(snippet);
                return;
            }
            if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
            {
                PasteItemToFocusedApp(selectedItem);
            }
        }
        private async void PasteItemToFocusedApp(ClipboardItem item)
        {
            _viewModel.CopyClicked(item, asObject: false);
            _trayApplicationContext.HideMainWindow();
            await System.Threading.Tasks.Task.Delay(50);
            NativeMethods.SendCtrlV();
        }
        private async void PasteSnippet(SnippetItem snippet)
        {
            if (string.IsNullOrEmpty(snippet.Content)) return;
            if (SettingsLoader.Current.SuppressClipboardEvents)
                _trayApplicationContext.SuppressClipboardEvents(true);
            try
            {
                Clipboard.SetText(snippet.Content, TextDataFormat.UnicodeText);
            }
            finally
            {
                if (SettingsLoader.Current.SuppressClipboardEvents)
                {
                    System.Threading.Tasks.Task.Delay(80).ContinueWith(_ =>
                        _trayApplicationContext.SuppressClipboardEvents(false));
                }
            }
            _trayApplicationContext.HideMainWindow();
            await System.Threading.Tasks.Task.Delay(50);
            NativeMethods.SendCtrlV();
        }

        private void MessagesListBox_MouseWheel(object sender, MouseEventArgs e)
        {
            const int ScrollStep = 10;
            int currentTopIndex = MessagesListBox.TopIndex;
            if (e.Delta > 0)
            {
                currentTopIndex = Math.Max(0, currentTopIndex - ScrollStep);
            }
            else if (e.Delta < 0)
            {
                currentTopIndex = Math.Min(MessagesListBox.Items.Count - 1, currentTopIndex + ScrollStep);
            }
            MessagesListBox.TopIndex = currentTopIndex;
            ((HandledMouseEventArgs)e).Handled = true;
        }
        public void MessageAdd(ClipboardItem item)
        {
            // Snippet modundayken ListBox'a ekleme; cache'e eklendi, geri dönünce görünür
            if (_isSnippetMode) return;

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
            // Snippet modundayken görsel listeden kaldırma; RefreshList ile güncellenir
            if (_isSnippetMode) return;

            int removeIndex = _viewModel.GetIndexToRemove(MessagesListBox.Items.OfType<ClipboardItem>());
            if (removeIndex != -1)
            {
                MessagesListBox.Items.RemoveAt(removeIndex);
                if (MessagesListBox.Items.Count > 0)
                    MessagesListBox.TopIndex = SettingsLoader.Current.InvertClipboardHistoryListing ? 0 : MessagesListBox.Items.Count - 1;
            }
        }
        public void MessageRemoveItem(ClipboardItem item)
        {
            // Snippet modundayken görseli değiştirme
            if (_isSnippetMode) return;

            if (MessagesListBox.Items.Contains(item))
            {
                MessagesListBox.Items.Remove(item);
                MessagesListBox.Refresh();
            }
        }
        public void RefreshList(bool keepSelection = false)
        {
            if (_isSnippetMode)
            {
                MessagesListBox.BeginUpdate();
                try { RefreshSnippetList(); }
                finally { MessagesListBox.EndUpdate(); }
                return;
            }
            MessagesListBox.BeginUpdate();
            try
            {
                ClipboardItem previouslySelected = null;
                if (keepSelection)
                    previouslySelected = MessagesListBox.SelectedItem as ClipboardItem;
                MessagesListBox.Items.Clear();
                foreach (var item in _viewModel.GetDisplayList(_currentSearchTerm))
                    MessagesListBox.Items.Add(item);
                if (keepSelection && previouslySelected != null)
                    MessagesListBox.SelectedItem = previouslySelected;
            }
            finally
            {
                MessagesListBox.EndUpdate();
                UpdateStatusLabel();
            }
        }
        private void MessagesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MessagesListBox.SelectedIndex < 0) return;

            if (!_isSnippetMode)
            {
                OpenDetail(MessagesListBox.SelectedIndex);
                UpdateMenuStates();
            }

            if (!SettingsLoader.Current.FocusDetailWindow)
                poisonTextBox1_search.Focus();
        }
        private void OpenDetail(int index)
        {
            if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
            {
                Rectangle currentBounds =
                    _detailManager.GetActiveForm()?.Bounds ?? Rectangle.Empty;

                _detailManager.ShowDetail(selectedItem);
            }
        }
        private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int index = MessagesListBox.IndexFromPoint(e.Location);
            if (index < 0) return;

            MessagesListBox.SelectedIndex = index; // sadece seçim
        }
        private void MessagesListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (_isSnippetMode)
            {
                DrawingHelper.RenderSnippetItem(e, MessagesListBox);
                return;
            }
            DrawingHelper.RenderClipboardItem(e, MessagesListBox, _currentSearchTerm, _viewModel);
        }
        private void OpenDetailForIndex(int index)
        {
            if (index < 0) return;
            MessagesListBox.SelectedIndex = index;
            if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
            {
                Rectangle currentBounds = _detailManager.GetActiveForm()?.Bounds ?? Rectangle.Empty;
                _detailManager.ShowDetail(selectedItem);
            }
        }
        #endregion

        #region SEARCH OPERATIONS
        private void pictureBox2_searchSettings_Click(object sender, EventArgs e)
        {

            panel_searchSettings.Visible = !panel_searchSettings.Visible;
            pictureBox2_searchSettings.BackColor = panel_searchSettings.Visible ? AppColors.GetButtonActiveColor() : Color.Transparent;
        }
        private void poisonToggle2_caseSens_CheckedChanged(object sender, EventArgs e)
        {
            _viewModel.SetSearchOptions(poisonToggle1_regex.Checked, poisonToggle2_caseSens.Checked);
            RefreshList();
        }
        private void poisonToggle1_regex_CheckedChanged(object sender, EventArgs e)
        {
            _viewModel.SetSearchOptions(poisonToggle1_regex.Checked, poisonToggle2_caseSens.Checked);
            poisonTextBox1_search_TextChanged(null, null);
        }
        private void poisonTextBox1_search_TextChanged(object sender, EventArgs e)
        {
            _currentSearchTerm = poisonTextBox1_search.Text;
            CloseDetailFormIfAvaible();
            MessagesListBox.ClearSelected();
            RefreshList();
        }
        private void poisonTextBox1_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (poisonTextBox1_search.HandleWordDeletion(e))
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
                    case Keys.PageDown:
                        int lastIndex = MessagesListBox.Items.Count - 1;
                        MessagesListBox.SelectedIndex = lastIndex;
                        MessagesListBox.TopIndex = lastIndex;
                        e.Handled = e.SuppressKeyPress = true;
                        break;

                    case Keys.PageUp:
                        int firstIndex = 0;
                        MessagesListBox.SelectedIndex = firstIndex;
                        MessagesListBox.TopIndex = firstIndex;
                        e.Handled = e.SuppressKeyPress = true;
                        break;
                    case Keys.Delete:
                        if (e.Modifiers == Keys.Shift || poisonTextBox1_search.Text.Length == 0)
                        {
                            if (MessagesListBox.SelectedItem != null)
                            {
                                delToolStripMenuItem_Click(sender, e);
                                e.Handled = e.SuppressKeyPress = true;
                            }
                        }
                        break;
                    case Keys.Enter:
                        if (_isSnippetMode && MessagesListBox.SelectedItem is SnippetItem snippet)
                        {
                            PasteSnippet(snippet);
                        }
                        else if (MessagesListBox.SelectedItem is ClipboardItem selectedItem)
                        {
                            if (SettingsLoader.Current.QuickPasteOnEnter)
                            {
                                PasteItemToFocusedApp(selectedItem);
                            }
                            else
                            {
                                _viewModel.CopyClicked(selectedItem, asObject: false);
                                _trayApplicationContext.HideMainWindow();
                            }
                        }

                        e.Handled = e.SuppressKeyPress = true;
                        break;

                }
            }
        }
        private void poisonTextBox1_search_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
        public void FocusSearchBox() => poisonTextBox1_search.Focus();
        public void ClearSearchBox() => poisonTextBox1_search.Text = string.Empty;
        #endregion

        #region CONTEXT MENU & ITEM ACTIONS
        private void delToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isSnippetMode)
            {
                if (!(MessagesListBox.SelectedItem is SnippetItem selectedSnippet)) return;
                var confirmResult = MessageBox.Show($"Delete snippet '{selectedSnippet.Name}'?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult != DialogResult.Yes) return;
                SnippetLoader.Remove(selectedSnippet);
                RefreshList();
                return;
            }
            if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;
            CloseDetailFormIfAvaible();
            _viewModel.DeleteItem(selected);
            RefreshList();
            poisonTextBox1_search.Focus();
        }
        private void cancelStripMenuItem1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1?.Close();
        }
        public void CallDeleteFromDetailWindow(object sender, EventArgs e) => delToolStripMenuItem_Click(sender, e);
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var pos = MessagesListBox.PointToClient(Cursor.Position);
            int index = MessagesListBox.IndexFromPoint(pos);
            if (index < 0) { e.Cancel = true; return; }

            if (_isSnippetMode)
            {
                // Snippet modunda sadece ilgili menü öğeleri görünsün
                copyToolStripMenuItem.Visible = false;
                openToolStripMenuItem.Visible = false;
                saveToolStripMenuItem.Visible = false;
                pinUnpinToolStripMenuItem.Visible = false;
                addTagToolStripMenuItem.Visible = false;
                saveAsSnippetToolStripMenuItem.Visible = false;
                toolStripSeparator1.Visible = false;
                delToolStripMenuItem.Text = "Delete Snippet";
                delToolStripMenuItem.Visible = true;
                cancelStripMenuItem1.Visible = true;
                MessagesListBox.SelectedIndex = index;
                return;
            }

            // Normal clipboard modu
            copyToolStripMenuItem.Visible = true;
            openToolStripMenuItem.Visible = true;
            saveToolStripMenuItem.Visible = true;
            pinUnpinToolStripMenuItem.Visible = true;
            addTagToolStripMenuItem.Visible = true;
            saveAsSnippetToolStripMenuItem.Visible = true;
            toolStripSeparator1.Visible = true;
            delToolStripMenuItem.Text = "Delete";
            delToolStripMenuItem.Visible = true;
            cancelStripMenuItem1.Visible = true;
            OpenDetailForIndex(index);
            UpdateMenuStates();
        }
        public void CopyCliked(bool asObject = false)
        {
            if (MessagesListBox.SelectedIndices.Count == 0) return;
            CloseDetailFormIfAvaible();
            ClipboardItem selectedItem = MessagesListBox.SelectedItem as ClipboardItem;
            _viewModel.CopyClicked(selectedItem, asObject);
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
            var state = _viewModel.GetMenuState(selected);
            copyToolStripMenuItem.Enabled = state.CanCopy;
            openToolStripMenuItem.Enabled = state.CanOpen;
            saveToolStripMenuItem.Enabled = state.CanSave;
            pinUnpinToolStripMenuItem.Checked = state.IsPinned;
            pinUnpinToolStripMenuItem.Text = state.PinText;
        }
        private void pcbox_clearClipboard_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the clipboard history?", "Clear Clipboard", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) _trayApplicationContext.ClearClipboard();
        }
        private void addTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;
            string currentTags = selected.Tags != null && selected.Tags.Count > 0
                ? string.Join(", ", selected.Tags)
                : "";
            string input = ShowInputDialog("Enter tags (comma separated):", "Add Tags", currentTags);
            if (input == null) return;
            var tags = input.Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            _viewModel.SetTags(selected, tags);
            RefreshTagFilter();
            RefreshList(true);
        }
        private void poisonDropDownButton_tagFilter_Click(object sender, EventArgs e)
        {
            poisonDropDownButton_tagFilter.OpenDropDown();
        }
        private void TagFilterMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                string tag = item.Tag as string; // null = "All Tags"
                _viewModel.SetTagFilter(tag);
                poisonDropDownButton_tagFilter.Text = tag ?? "All Tags";
                RefreshList();
            }
        }
        public void RefreshTagFilter()
        {
            string currentFilter = poisonDropDownButton_tagFilter.Text;
            contextMenuStrip_tagFilter.Items.Clear();

            var allItem = new ToolStripMenuItem("All Tags") { Tag = null };
            allItem.Click += TagFilterMenuItem_Click;
            if (currentFilter == "All Tags" || string.IsNullOrEmpty(currentFilter))
                allItem.Font = new System.Drawing.Font(allItem.Font, System.Drawing.FontStyle.Bold);
            contextMenuStrip_tagFilter.Items.Add(allItem);

            var tags = _viewModel.GetAllTags();
            if (tags.Count > 0)
                contextMenuStrip_tagFilter.Items.Add(new ToolStripSeparator());

            foreach (var tag in tags)
            {
                var menuItem = new ToolStripMenuItem(tag) { Tag = tag };
                menuItem.Click += TagFilterMenuItem_Click;
                if (currentFilter == tag)
                    menuItem.Font = new System.Drawing.Font(menuItem.Font, System.Drawing.FontStyle.Bold);
                contextMenuStrip_tagFilter.Items.Add(menuItem);
            }
        }
        #endregion

        #region NAVIGATION & DIALOGS
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _suppressAutoHide = true;
            using (var f = new SettingsForm(this))
            {
                f.StartPosition = FormStartPosition.CenterParent;
                f.TopMost = true;
                f.ShowDialog(this); // parent VERME
            }
            _suppressAutoHide = false;
        }
        private async void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkUpdateToolStripMenuItem.Enabled = false;

            try
            {
                var updateService = _trayApplicationContext.GetUpdateService();
                var updateWrapper = await updateService.CheckForUpdateAsync(false);

                if (!updateWrapper.Success)
                {
                    MessageBox.Show(
                        $"Failed to check updates:\n{updateWrapper.ErrorMessage}",
                        "Update Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else if (updateWrapper.UpdateInfo != null)
                {
                    var info = updateWrapper.UpdateInfo;
                    var result = MessageBox.Show(
                        $"New version v{info.Version} ({info.BuildNumber}) is available!\n\nNotes: {info.Notes}\n\nDownload now?",
                        "Update Found",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                        await updateService.DownloadAndRunUpdateAsync();
                }
                else
                {
                    MessageBox.Show(
                        "Your application is up to date.",
                        "No Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unexpected error:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                checkUpdateToolStripMenuItem.Enabled = true;
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => ShowHtmlDialog(AboutHtml.GetTitle(), AboutHtml.GetHtml());
        private void phoneSyncToolStripMenuItem_Click(object sender, EventArgs e) => ShowUnderDevelopmentDialog("Phone Sync");
        private void ShowUnderDevelopmentDialog(string featureName) => ShowHtmlDialog(UnderDevelopmentHtml.GetTitle(), UnderDevelopmentHtml.GetHtml(featureName));
        private void ShowHtmlDialog(string title, string html)
        {
            _suppressAutoHide = true;
            using (var dlg = new WebDialog(title, html))
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.TopMost = true;
                dlg.ShowDialog(this);
            }
            _suppressAutoHide = false;
        }
        #endregion

        #region UI STATE HELPERS
        private void pcbox_togglePrivacy_Click(object sender, EventArgs e)
        {
            _trayApplicationContext.TogglePrivacyMode();
            UpdatePrivacyStatusUI();
        }
        public void UpdateDetailWindowKeyPreview(bool focusEnabled)
        {
            _detailManager.SetKeyPreview(focusEnabled);
        }
        public void UpdateCheckUpdateNowBtnText(string newString) => checkUpdateToolStripMenuItem.Text = newString;
        public void UpdateTaskbarVisibility(bool visible) => this.ShowInTaskbar = visible;
        public void CloseDetailFormIfAvaible() => _detailManager.CloseAll();
        public void ResetFormPositionAndSize() => _layoutManager.ResetToDefault();
        public void CheckAndUpdateTopMostImage()
        {
            pcbox_topMost.Image = _viewModel.IsLocked
        ? Properties.Resources.lock_40px
        : Properties.Resources.unlock_40px;
            pcbox_topMost.BackColor = _viewModel.IsLocked ? AppColors.GetButtonActiveColor() : Color.Transparent;
        }
        public void UpdatePrivacyStatusUI()
        {
            bool isActive = _trayApplicationContext.IsPrivacyModeActive;
            pcbox_togglePrivacy.Image = isActive ? Properties.Resources.invisible_40px : Properties.Resources.eye_40px;
            pcbox_togglePrivacy.BackColor = isActive ? AppColors.GetButtonActiveColor() : Color.Transparent;
            RefreshList();

        }
        public void UpdateStatusLabel()
        {
            toolStripStatusLabel1.Text = _viewModel.GetStatusText();
        }
        private void pcbox_topMost_Click(object sender, EventArgs e)
        {
            this.TopMost = _viewModel.ToggleLock(SettingsLoader.Current.AlwaysTopMost);
            CheckAndUpdateTopMostImage();
        }
        #endregion

        #region MANUAL RESIZE (BORDERLESS SUPPORT)
        protected override void WndProc(ref Message m)
        {
            if (_trayApplicationContext.ApplicationExiting)
                return;
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if ((int)m.Result == HTCLIENT)
                {
                    Point cursor = PointToClient(Cursor.Position);

                    IntPtr hit = ResizeHitTestHelper.GetHitTest(this, cursor, 8);
                    if (hit != IntPtr.Zero)
                    {
                        m.Result = hit;
                        return;
                    }
                }
                return;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region SNIPPET MODE
        private void btnSnippetTab_Click(object sender, EventArgs e)
        {
            _isSnippetMode = !_isSnippetMode;
            snippetsToolStripMenuItem.Text = _isSnippetMode ? "< Clipboard" : "Snippets";
            snippetsToolStripMenuItem.Font = _isSnippetMode
                ? new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold)
                : new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);

            btnSnippetAdd.Visible = _isSnippetMode;
            btnSnippetEdit.Visible = _isSnippetMode;
            btnSnippetDelete.Visible = _isSnippetMode;
            poisonDropDownButton_tagFilter.Visible = !_isSnippetMode;
            pcbox_clearClipboard.Visible = !_isSnippetMode;

            // Snippet modunda arama kutusu placeholder'ı güncelle
            poisonTextBox1_search.WaterMark = _isSnippetMode ? "Search snippets..." : "Search...";
            poisonTextBox1_search.Text = string.Empty;
            _currentSearchTerm = string.Empty;

            CloseDetailFormIfAvaible();
            RefreshList();
        }

        private void RefreshSnippetList()
        {
            MessagesListBox.Items.Clear();
            var snippets = SnippetLoader.Items;
            string search = _currentSearchTerm;
            foreach (var s in snippets)
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    if (s.Name?.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0 &&
                        s.Content?.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                }
                MessagesListBox.Items.Add(s);
            }
            toolStripStatusLabel1.Text = $"Snippets: {MessagesListBox.Items.Count}";
        }

        private void btnSnippetAdd_Click(object sender, EventArgs e)
        {
            string name = ShowInputDialog("Snippet name:", "New Snippet", "");
            if (string.IsNullOrWhiteSpace(name)) return;
            string content = ShowMultilineInputDialog("Snippet content (supports multiple lines):", "New Snippet", "");
            if (content == null) return;
            SnippetLoader.Add(new SnippetItem(name, content));
            RefreshList();
        }

        private void btnSnippetEdit_Click(object sender, EventArgs e)
        {
            if (!(MessagesListBox.SelectedItem is SnippetItem selected)) return;
            string name = ShowInputDialog("Snippet name:", "Edit Snippet", selected.Name);
            if (string.IsNullOrWhiteSpace(name)) return;
            string content = ShowMultilineInputDialog("Snippet content (supports multiple lines):", "Edit Snippet", selected.Content);
            if (content == null) return;
            SnippetLoader.Update(selected, name, content);
            RefreshList(true);
        }

        private void btnSnippetDelete_Click(object sender, EventArgs e)
        {
            if (!(MessagesListBox.SelectedItem is SnippetItem selected)) return;
            var result = MessageBox.Show($"Delete snippet '{selected.Name}'?", "Delete", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            SnippetLoader.Remove(selected);
            RefreshList();
        }

        private void saveAsSnippetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(MessagesListBox.SelectedItem is ClipboardItem selected)) return;
            if (string.IsNullOrEmpty(selected.Content)) return;
            string name = ShowInputDialog("Snippet name:", "Save as Snippet",
                selected.Title?.Length > 50 ? selected.Title.Substring(0, 50) : selected.Title ?? "");
            if (string.IsNullOrWhiteSpace(name)) return;
            SnippetLoader.Add(new SnippetItem(name, selected.Content));
            MessageBox.Show("Snippet saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region THEME
        public void ThemeChanged()
        {
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            this.MessagesListBox.BackColor = this.BackColor;
            _detailManager.ApplyThemeToDetailWindows();
        }
        #endregion

        private string ShowInputDialog(string prompt, string title, string defaultValue)
        {
            _suppressAutoHide = true;
            try
            {
                using (var form = new Form())
                {
                    form.Text = title;
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.ClientSize = new Size(360, 115);
                    form.TopMost = true;

                    var label = new Label { Text = prompt, Left = 10, Top = 12, AutoSize = true };
                    var textBox = new TextBox { Left = 10, Top = 36, Width = 338, Text = defaultValue ?? "" };
                    textBox.SelectAll();
                    var btnOk = new Button { Text = "OK", Left = 192, Top = 76, Width = 75, DialogResult = DialogResult.OK };
                    var btnCancel = new Button { Text = "Cancel", Left = 273, Top = 76, Width = 75, DialogResult = DialogResult.Cancel };

                    form.Controls.AddRange(new Control[] { label, textBox, btnOk, btnCancel });
                    form.AcceptButton = btnOk;
                    form.CancelButton = btnCancel;
                    form.ActiveControl = textBox;

                    return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
                }
            }
            finally
            {
                _suppressAutoHide = false;
            }
        }

        private string ShowMultilineInputDialog(string prompt, string title, string defaultValue)
        {
            _suppressAutoHide = true;
            try
            {
                using (var form = new Form())
                {
                    form.Text = title;
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.ClientSize = new Size(420, 260);
                    form.TopMost = true;

                    var label = new Label { Text = prompt, Left = 10, Top = 10, AutoSize = true };
                    var textBox = new TextBox
                    {
                        Left = 10,
                        Top = 32,
                        Width = 398,
                        Height = 180,
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical,
                        Text = defaultValue ?? "",
                        AcceptsReturn = true,
                        AcceptsTab = false
                    };
                    var hintLabel = new Label
                    {
                        Text = "Tip: Press Enter for new line. Ctrl+Enter to confirm.",
                        Left = 10,
                        Top = 218,
                        AutoSize = true,
                        ForeColor = System.Drawing.Color.Gray,
                        Font = new System.Drawing.Font("Segoe UI", 7.5F)
                    };
                    var btnOk = new Button { Text = "OK", Left = 253, Top = 218, Width = 75, DialogResult = DialogResult.OK };
                    var btnCancel = new Button { Text = "Cancel", Left = 333, Top = 218, Width = 75, DialogResult = DialogResult.Cancel };

                    // Ctrl+Enter ile onayla
                    textBox.KeyDown += (s, e) =>
                    {
                        if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
                        {
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                    };

                    form.Controls.AddRange(new Control[] { label, textBox, hintLabel, btnOk, btnCancel });
                    form.CancelButton = btnCancel;
                    form.ActiveControl = textBox;
                    textBox.SelectAll();

                    return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
                }
            }
            finally
            {
                _suppressAutoHide = false;
            }
        }

        public void ApplyFormBehaviorSettings(bool showWarnings = false)
        {
            this.TopMost = SettingsLoader.Current.AlwaysTopMost;
            this.CheckAndUpdateTopMostImage();
            this.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;
          
            _suppressAutoHide = false;
        }
    }
}