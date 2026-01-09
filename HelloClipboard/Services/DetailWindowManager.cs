using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
	/// <summary>
	/// Manages the detail preview windows for clipboard items.
	/// Handles specialized forms for text and images, ensuring proper positioning and visibility.
	/// </summary>
	public class DetailWindowManager
	{
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
		}

		/// <summary>
		/// Displays the detail window for a specific clipboard item.
		/// Switches between Text and Image detail forms based on the item type.
		/// </summary>
		public void ShowDetail(ClipboardItem item, Rectangle previousBounds = default)
		{
			if (item == null) return;

			Form targetForm;

			// 1. Dosya Tipi Kontrolü (Yeni)
			if (item.ItemType == ClipboardItemType.Path)
			{
				if (_detailFileForm == null || _detailFileForm.IsDisposed)
				{
					_detailFileForm = new ClipDetailFile(_owner, item);
					_detailFileForm.Deactivate += _deactivateHandler;
					_detailFileForm.Owner = _owner;
				}
				else
				{
					_detailFileForm.UpdateItem(item);
				}

				targetForm = _detailFileForm;
				_detailTextForm?.Hide();
				_detailImageForm?.Hide();
			}
			// 2. Resim Tipi Kontrolü
			else if (item.ItemType == ClipboardItemType.Image)
			{
				if (_detailImageForm == null || _detailImageForm.IsDisposed)
				{
					_detailImageForm = new ClipDetailImage(_owner, item);
					_detailImageForm.Deactivate += _deactivateHandler;
					_detailImageForm.Owner = _owner;
				}
				else
				{
					_detailImageForm.UpdateItem(item);
				}

				targetForm = _detailImageForm;
				_detailTextForm?.Hide();
				_detailFileForm?.Hide(); // Dosya formunu gizle
			}
			// 3. Metin veya Diğer Tipler
			else
			{
				if (_detailTextForm == null || _detailTextForm.IsDisposed)
				{
					_detailTextForm = new ClipDetailText(_owner, item);
					_detailTextForm.Deactivate += _deactivateHandler;
					_detailTextForm.Owner = _owner;
				}
				else
				{
					_detailTextForm.UpdateItem(item);
				}

				targetForm = _detailTextForm;
				_detailImageForm?.Hide();
				_detailFileForm?.Hide(); // Dosya formunu gizle
			}

			_activeForm = targetForm;

			// If specific bounds are provided (e.g., window was already open), maintain them
			if (!previousBounds.IsEmpty)
			{
				targetForm.StartPosition = FormStartPosition.Manual;
				targetForm.Bounds = previousBounds;
			}
			else
			{
				PositionFormNextToOwner(targetForm);
			}

			// Sync "Always on Top" setting with the main window
			targetForm.TopMost = _owner.TopMost;

			if (!targetForm.Visible) targetForm.Show();
		}

		/// <summary>
		/// Calculates the optimal position for the detail window relative to the main window.
		/// Tries to place it: Right -> Left -> Bottom -> Top -> Center (Fallback).
		/// </summary>
		public void PositionFormNextToOwner(Form detailForm)
		{
			if (detailForm == null || detailForm.IsDisposed) return;

			var mainRect = _owner.Bounds;
			var screen = Screen.FromControl(_owner).WorkingArea;
			int padding = 1;
			Point location;

			// Try placing to the RIGHT of the main window
			if (mainRect.Right + detailForm.Width + padding <= screen.Right)
				location = new Point(mainRect.Right + padding, mainRect.Top);
			// Try placing to the LEFT
			else if (mainRect.Left - detailForm.Width - padding >= screen.Left)
				location = new Point(mainRect.Left - detailForm.Width - padding, mainRect.Top);
			// Try placing to the BOTTOM
			else if (mainRect.Bottom + detailForm.Height + padding <= screen.Bottom)
				location = new Point(mainRect.Left, mainRect.Bottom + padding);
			// Try placing to the TOP
			else if (mainRect.Top - detailForm.Height - padding >= screen.Top)
				location = new Point(mainRect.Left, mainRect.Top - detailForm.Height - padding);
			// Fallback: Center of the current screen
			else
				location = new Point(
					Math.Max(screen.Left, screen.Left + (screen.Width - detailForm.Width) / 2),
					Math.Max(screen.Top, screen.Top + (screen.Height - detailForm.Height) / 2)
				);

			detailForm.StartPosition = FormStartPosition.Manual;
			detailForm.Location = location;
		}

		/// <summary>
		/// Hides all open detail windows and clears the active reference.
		/// </summary>
		public void CloseAll()
		{
			_detailTextForm?.Hide();
			_detailImageForm?.Hide();
			_detailFileForm?.Hide();
			_activeForm = null;
		}

		public Form GetActiveForm() => _activeForm;
		public bool IsAnyVisible() => _activeForm != null && _activeForm.Visible;
	}
}