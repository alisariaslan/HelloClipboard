using HelloClipboard.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
    /// <summary>
    /// Manages the layout, persistence, and behavior of the main form.
    /// Handles window geometry, snapping, and synchronization with detail windows.
    /// </summary>
    public class FormLayoutManager
    {
        private readonly Form _form;
        private readonly DetailWindowManager _detailManager; private Rectangle _restoreBounds;


        private bool _isLoaded;
        private FormWindowState _previousWindowState = FormWindowState.Normal;
        public bool IsLoaded => _isLoaded;

        public FormLayoutManager(Form form, DetailWindowManager detailManager)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _detailManager = detailManager;
        }

        #region Lifecycle

        public void OnLoad()
        {
            _form.StartPosition = FormStartPosition.Manual;
            FormPersistence.ApplyStoredGeometry(_form);

            _restoreBounds = _form.Bounds; // 🔥 EKLE

            _form.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;

            if (SettingsLoader.Current.AlwaysTopMost)
                _form.TopMost = true;

            _isLoaded = true;
        }


        public void OnShown()
        {
            // Placeholder for future logic upon form display
        }

        public void OnClosing()
        {
            if (_isLoaded)
            {
                // Save current window geometry before closing
                FormPersistence.SaveGeometry(_form);
            }

            // Ensure all child/detail windows are closed
            _detailManager?.CloseAll();
        }

        #endregion

        #region Window Events

        public void OnResize()
        {
            if (!_isLoaded)
                return;

            if (_form.WindowState == FormWindowState.Normal)
            {
                // 🔥 gerçek restore edilecek ölçü BURASI
                _restoreBounds = _form.Bounds;
                FormPersistence.SaveGeometry(_form);
            }

            _previousWindowState = _form.WindowState;
            RepositionDetailIfNeeded();
        }



        public void ToggleMaximize()
        {
            if (_form.WindowState == FormWindowState.Maximized)
            {
                _form.WindowState = FormWindowState.Normal;
                _form.Bounds = _restoreBounds; // 🔥 esas nokta
            }
            else
            {
                _restoreBounds = _form.Bounds;
                _form.WindowState = FormWindowState.Maximized;
            }
        }


        public void OnMove()
        {
            if (!_isLoaded)
                return;

            if (_form.WindowState != FormWindowState.Normal)
                return;

            _form.Location = WindowHelper.GetSnappedLocation(_form);
            FormPersistence.SaveGeometry(_form);

            RepositionDetailIfNeeded();
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Ensures that the active detail window follows the main window's position during movement or resizing.
        /// </summary>
        private void RepositionDetailIfNeeded()
        {
            if (_detailManager != null && _detailManager.IsAnyVisible())
            {
                _detailManager.PositionFormNextToOwner(
                    _detailManager.GetActiveForm()
                );
            }
        }

        /// <summary>
        /// Resets the main window to its factory default size and position.
        /// </summary>
        public void ResetToDefault()
        {
            _form.Size = new Size(480, 720);
            _form.StartPosition = FormStartPosition.CenterScreen;
            _form.WindowState = FormWindowState.Normal;
        }

        #endregion
    }
}