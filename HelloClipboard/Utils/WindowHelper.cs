using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloClipboard.Utils
{
	public static class WindowHelper
	{
		public static int CalculateSnap(int pos, int size, int screenStart, int screenEnd, int distance)
		{
			if (Math.Abs(pos - screenStart) < distance) return screenStart;
			if (Math.Abs((pos + size) - screenEnd) < distance) return screenEnd - size;
			return pos;
		}

		public static Point GetSnappedLocation(Form form, int snapDistance = 20)
		{
			var screen = Screen.FromControl(form).WorkingArea;
			int newX = CalculateSnap(form.Left, form.Width, screen.Left, screen.Right, snapDistance);
			int newY = CalculateSnap(form.Top, form.Height, screen.Top, screen.Bottom, snapDistance);

			return new Point(newX, newY);
		}
	}
}
