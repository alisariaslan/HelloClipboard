using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
	public class HotkeyService : IDisposable
	{
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private const int WM_HOTKEY = 0x0312;
		private readonly HotkeyWindow _window;
		private readonly int _hotkeyId = 1001;

		public bool IsRegistered { get; private set; }
		public event Action HotkeyPressed;

		public HotkeyService()
		{
			_window = new HotkeyWindow();
			_window.HotkeyReceived += () => HotkeyPressed?.Invoke();
		}

		public bool Register(Keys modifiers, Keys key)
		{
			Unregister(); 

			uint modFlags = 0;
			if (modifiers.HasFlag(Keys.Control)) modFlags |= 0x0002;
			if (modifiers.HasFlag(Keys.Alt)) modFlags |= 0x0001;
			if (modifiers.HasFlag(Keys.Shift)) modFlags |= 0x0004;
			if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin)) modFlags |= 0x0008;

			IsRegistered = RegisterHotKey(_window.Handle, _hotkeyId, modFlags, (uint)key);
			return IsRegistered;
		}

		public void Unregister()
		{
			if (IsRegistered && _window.Handle != IntPtr.Zero)
			{
				UnregisterHotKey(_window.Handle, _hotkeyId);
				IsRegistered = false;
			}
		}

		public void Dispose()
		{
			Unregister();
			_window.Dispose();
		}

		private class HotkeyWindow : NativeWindow, IDisposable
		{
			public event Action HotkeyReceived;
			public HotkeyWindow() => CreateHandle(new CreateParams());

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == WM_HOTKEY) HotkeyReceived?.Invoke();
				base.WndProc(ref m);
			}

			public void Dispose() => DestroyHandle();
		}
	}
}