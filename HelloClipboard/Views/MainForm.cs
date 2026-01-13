using HelloClipboard.Constants;
using HelloClipboard.Html;
using HelloClipboard.Services;
using HelloClipboard.Utils;
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

        public MainForm(TrayApplicationContext trayApplicationContext)
        {
            InitializeComponent();
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            this.Text = $"{Application.ProductName}";
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
            MessagesListBox.ItemHeight = 24;
            MessagesListBox.DrawItem += MessagesListBox_DrawItem;
            MessagesListBox.Resize += (s, e) => MessagesListBox.Invalidate();
            MessagesListBox.SelectedIndexChanged += MessagesListBox_SelectedIndexChanged;
            MessagesListBox.MouseClick += MessagesListBox_MouseClick;
            MessagesListBox.MouseWheel += MessagesListBox_MouseWheel;
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
        public void RefreshList(bool keepSelection = false)
        {
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
            if (MessagesListBox.SelectedIndex >= 0)
            {
                OpenDetailForIndex(MessagesListBox.SelectedIndex);
                if (!SettingsLoader.Current.FocusDetailWindow)
                {
                    poisonTextBox1_search.Focus();
                }
                UpdateMenuStates();
            }
        }
        private void MessagesListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int index = MessagesListBox.IndexFromPoint(e.Location);
            OpenDetailForIndex(index);
            if (!SettingsLoader.Current.FocusDetailWindow)
            {
                poisonTextBox1_search.Focus();
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
            var updateService = _trayApplicationContext.GetUpdateService();
            var update = await updateService.CheckForUpdateAsync(false);
            if (update != null)
            {
                var result = MessageBox.Show($"New version v{update.Version} ({update.BuildNumber}) is available!\n\nNotes: {update.Notes}\n\nDownload now?", "Update Found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
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
            _suppressAutoHide = true;
            using (var dlg = new InfoDialog(title, html))
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

        #region THEME
        public void ThemeChanged()
        {
            ThemeHelper.ApplySavedThemeToForm(this, poisonStyleManager1);
            _detailManager.ApplyThemeToDetailWindows();
        }
        #endregion
    }
}