using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
	public class DetailWindowManager
	{
		private readonly MainForm _owner;
		private readonly EventHandler _deactivateHandler;
		private Form _activeForm;
		private ClipDetailText _detailTextForm;
		private ClipDetailImage _detailImageForm;

		public DetailWindowManager(MainForm owner, EventHandler deactivateHandler)
		{
			_owner = owner;
			_deactivateHandler = deactivateHandler;
		}

		public void ShowDetail(ClipboardItem item, Rectangle previousBounds = default)
		{
			if (item == null) return;

			Form targetForm;
			if (item.ItemType == ClipboardItemType.Image)
			{
				if (_detailImageForm == null || _detailImageForm.IsDisposed)
				{
					_detailImageForm = new ClipDetailImage(_owner, item);
					_detailImageForm.Deactivate += _deactivateHandler;
					_detailImageForm.Owner = _owner;
				}
				else _detailImageForm.UpdateItem(item);

				targetForm = _detailImageForm;
				_detailTextForm?.Hide();
			}
			else
			{
				if (_detailTextForm == null || _detailTextForm.IsDisposed)
				{
					_detailTextForm = new ClipDetailText(_owner, item);
					_detailTextForm.Deactivate += _deactivateHandler;
					_detailTextForm.Owner = _owner;
				}
				else _detailTextForm.UpdateItem(item);

				targetForm = _detailTextForm;
				_detailImageForm?.Hide();
			}

			_activeForm = targetForm;

			if (!previousBounds.IsEmpty)
			{
				targetForm.StartPosition = FormStartPosition.Manual;
				targetForm.Bounds = previousBounds;
			}
			else PositionFormNextToOwner(targetForm);

			targetForm.TopMost = _owner.TopMost;
			if (!targetForm.Visible) targetForm.Show();
		}

		public void PositionFormNextToOwner(Form detailForm)
		{
			if (detailForm == null || detailForm.IsDisposed) return;

			var mainRect = _owner.Bounds;
			var screen = Screen.FromControl(_owner).WorkingArea;
			int padding = 1;
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
					Math.Max(screen.Left, screen.Left + (screen.Width - detailForm.Width) / 2),
					Math.Max(screen.Top, screen.Top + (screen.Height - detailForm.Height) / 2)
				);

			detailForm.StartPosition = FormStartPosition.Manual;
			detailForm.Location = location;
		}

		public void CloseAll()
		{
			_detailTextForm?.Hide();
			_detailImageForm?.Hide();
			_activeForm = null;
		}

		public Form GetActiveForm() => _activeForm;
		public bool IsAnyVisible() => _activeForm != null && _activeForm.Visible;
	}
}