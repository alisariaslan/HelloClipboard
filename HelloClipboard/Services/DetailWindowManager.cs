using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
    public class DetailWindowManager
    {
        #region Fields & Constructor
        private readonly MainForm _owner;
        private readonly EventHandler _deactivateHandler;
        private Form _activeForm;
        private ClipDetailText _detailTextForm;
        private ClipDetailImage _detailImageForm;
        private ClipDetailFile _detailFileForm;
        public DetailWindowManager(MainForm owner, EventHandler deactivateHandler)
        {
            _owner = owner;
            _deactivateHandler = deactivateHandler;

            // Owner kapanırsa tüm detail formlar kesin olarak dispose edilir
            _owner.FormClosed += Owner_FormClosed;
        }
        #endregion

        #region PUBLIC API
        public void ShowDetail(ClipboardItem item)
        {
            if (item == null || _owner.IsDisposed)
                return;

            Form targetForm = null;

            switch (item.ItemType)
            {
                case ClipboardItemType.Path:
                    EnsureFileForm(item);
                    targetForm = _detailFileForm;
                    HideOthers(_detailTextForm, _detailImageForm);
                    break;

                case ClipboardItemType.Image:
                    EnsureImageForm(item);
                    targetForm = _detailImageForm;
                    HideOthers(_detailTextForm, _detailFileForm);
                    break;

                default:
                    EnsureTextForm(item);
                    targetForm = _detailTextForm;
                    HideOthers(_detailImageForm, _detailFileForm);
                    break;
            }

            if (!IsFormUsable(targetForm))
                return;

            _activeForm = targetForm;

            PositionFormNextToOwner(targetForm);

            bool ownerMaximized = _owner.WindowState == FormWindowState.Maximized;

            // Owner ilişkisi
            if (ownerMaximized)
            {
                targetForm.Owner = _owner;
                targetForm.TopMost = true;
            }
            else
            {
                targetForm.Owner = null;
                targetForm.TopMost = _owner.TopMost;
            }

            if (!targetForm.Visible)
            {
                if (ownerMaximized)
                    targetForm.Show(_owner); // owner ile göster
                else
                    targetForm.Show();
            }

            if (SettingsLoader.Current.FocusDetailWindow)
                targetForm.Activate();

        }
        public void CloseAll()
        {
            HideSafe(_detailTextForm);
            HideSafe(_detailImageForm);
            HideSafe(_detailFileForm);
            _activeForm = null;
        }
        public void SetKeyPreview(bool enabled)
        {
            SetKeyPreviewSafe(_detailTextForm, enabled);
            SetKeyPreviewSafe(_detailImageForm, enabled);
            SetKeyPreviewSafe(_detailFileForm, enabled);
        }
        public void ApplyThemeToDetailWindows()
        {
            if (IsFormUsable(_detailTextForm))
                _detailTextForm.RefreshTheme();
            if (IsFormUsable(_detailFileForm))
                _detailFileForm.RefreshTheme();
            if (IsFormUsable(_detailImageForm))
                _detailImageForm.RefreshTheme();
        }
        public Form GetActiveForm() => IsFormUsable(_activeForm) ? _activeForm : null;
        public bool IsAnyVisible() => IsFormUsable(_activeForm) && _activeForm.Visible;
        #endregion

        #region ENSURE FORMS
        private void EnsureTextForm(ClipboardItem item)
        {
            if (!IsFormUsable(_detailTextForm))
            {
                _detailTextForm = new ClipDetailText(_owner, item);
                _detailTextForm.RefreshTheme();
                InitDetailForm(_detailTextForm);
            }
            else
            {
                _detailTextForm.UpdateItem(item);
            }
        }
        private void EnsureImageForm(ClipboardItem item)
        {
            if (!IsFormUsable(_detailImageForm))
            {
                _detailImageForm = new ClipDetailImage(_owner, item);
                _detailImageForm.RefreshTheme();
                InitDetailForm(_detailImageForm);
            }
            else
            {
                _detailImageForm.UpdateItem(item);
            }
        }
        private void EnsureFileForm(ClipboardItem item)
        {
            if (!IsFormUsable(_detailFileForm))
            {
                _detailFileForm = new ClipDetailFile(_owner, item);
                _detailFileForm.RefreshTheme();
                InitDetailForm(_detailFileForm);
            }
            else
            {
                _detailFileForm.UpdateItem(item);
            }
        }
        #endregion

        #region INIT & LIFECYCLE
        private void InitDetailForm(Form form)
        {
            form.Deactivate += _deactivateHandler;
            form.KeyDown += DetailForm_KeyDown;
            form.KeyPreview = SettingsLoader.Current.FocusDetailWindow;
            form.ShowInTaskbar = false;
        }
        private void Owner_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisposeAll();
        }
        private void DisposeAll()
        {
            DisposeSafe(ref _detailTextForm);
            DisposeSafe(ref _detailImageForm);
            DisposeSafe(ref _detailFileForm);
            _activeForm = null;
        }
        #endregion

        #region HELPERS (SAFE GUARDS)
        private static bool IsFormUsable(Form form)
        {
            return form != null && !form.IsDisposed;
        }
        private static void HideSafe(Form form)
        {
            if (IsFormUsable(form) && form.Visible)
                form.Hide();
        }
        private static void DisposeSafe<T>(ref T form) where T : Form
        {
            if (form == null) return;

            try
            {
                form.Hide();
                form.Dispose();
            }
            catch { }

            form = null;
        }
        private static void SetKeyPreviewSafe(Form form, bool enabled)
        {
            if (IsFormUsable(form))
                form.KeyPreview = enabled;
        }
        private static void HideOthers(params Form[] forms)
        {
            foreach (var form in forms)
                HideSafe(form);
        }
        #endregion

        #region POSITIONING
        public void PositionFormNextToOwner(Form detailForm)
        {
            if (!IsFormUsable(detailForm))
                return;

            var mainRect = _owner.Bounds;
            var screen = Screen.FromControl(_owner).WorkingArea;
            const int padding = 1;

            Point location;

            if (mainRect.Right + detailForm.Width + padding <= screen.Right)
                location = new Point(mainRect.Right + padding, mainRect.Top);
            else if (mainRect.Left - detailForm.Width - padding >= screen.Left)
                location = new Point(mainRect.Left - detailForm.Width - padding, mainRect.Top);
            else if (mainRect.Bottom + detailForm.Height + padding <= screen.Bottom)
                location = new Point(mainRect.Left, mainRect.Bottom + padding);
            else if (mainRect.Top - detailForm.Height - padding >= screen.Top)
                location = new Point(mainRect.Left, mainRect.Top - detailForm.Height - padding);
            else
                location = new Point(
                    screen.Left + (screen.Width - detailForm.Width) / 2,
                    screen.Top + (screen.Height - detailForm.Height) / 2
                );

            detailForm.StartPosition = FormStartPosition.Manual;
            detailForm.Location = location;
        }
        #endregion

        #region INPUT
        private void DetailForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!SettingsLoader.Current.FocusDetailWindow)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                _owner?.CallDeleteFromDetailWindow(sender, e);
                e.Handled = true;
            }
        }
        #endregion
    }
}
