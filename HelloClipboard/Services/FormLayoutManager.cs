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
        private readonly DetailWindowManager _detailManager;

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
            // CRITICAL: Designer settings like CenterScreen will override stored coordinates.
            // To ensure persistence works, we must force the StartPosition to Manual first.
            _form.StartPosition = FormStartPosition.Manual;

            // Apply previously saved window size and location
            FormPersistence.ApplyStoredGeometry(_form);

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

        public async void OnResize()
        {
            if (!_isLoaded)
                return;

            FormPersistence.SaveGeometry(_form);

            // Special handling for restoring from a Maximized state to ensure coordinates are applied correctly
            if (_form.WindowState == FormWindowState.Normal &&
                _previousWindowState == FormWindowState.Maximized)
            {
                await Task.Delay(10);
                FormPersistence.ApplyStoredGeometry(_form);
            }

            _previousWindowState = _form.WindowState;

            RepositionDetailIfNeeded();
        }

        public void OnMove()
        {
            if (!_isLoaded)
                return;

            // Handle window snapping (magnet effect) to screen edges
            _form.Location = WindowHelper.GetSnappedLocation(_form);

            // Persist the new location immediately (Crucial for unexpected shutdowns/crashes)
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