using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HelloClipboard.Utils
{
	public static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		public static bool IsCurrentProcessFocused()
		{
			IntPtr foregroundWindow = GetForegroundWindow();
			if (foregroundWindow == IntPtr.Zero) return false;

			GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
			return foregroundProcessId == (uint)Process.GetCurrentProcess().Id;
		}
	}
}
