using HelloClipboard.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
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
			FormPersistence.ApplyStoredGeometry(_form);

			_form.ShowInTaskbar = SettingsLoader.Current.ShowInTaskbar;

			if (SettingsLoader.Current.AlwaysTopMost)
				_form.TopMost = true;

			_isLoaded = true;
		}

		public void OnShown()
		{
			// Şu an boş ama ileride genişletilebilir
		}

		public void OnClosing()
		{
			if (_isLoaded)
			{
				FormPersistence.SaveGeometry(_form);
			}

			_detailManager?.CloseAll();
		}

		#endregion

		#region Window Events

		public async void OnResize()
		{
			if (!_isLoaded)
				return;

			FormPersistence.SaveGeometry(_form);

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

			_form.Location = WindowHelper.GetSnappedLocation(_form);
			RepositionDetailIfNeeded();
		}

		#endregion

		#region Helpers

		private void RepositionDetailIfNeeded()
		{
			if (_detailManager != null && _detailManager.IsAnyVisible())
			{
				_detailManager.PositionFormNextToOwner(
					_detailManager.GetActiveForm()
				);
			}
		}

		public void ResetToDefault()
		{
			_form.Size = new Size(480, 720);
			_form.StartPosition = FormStartPosition.CenterScreen;
			_form.WindowState = FormWindowState.Normal;
		}

		#endregion
	}
}
