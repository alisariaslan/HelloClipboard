using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelloClipboard.Services
{
    /// <summary>
    /// Provides system-wide hotkey registration by wrapping Win32 API calls.
    /// Uses a hidden NativeWindow to intercept WM_HOTKEY messages.
    /// Supports multiple hotkey registrations with individual callbacks.
    /// </summary>
    public class HotkeyService : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Windows Message ID for hotkey events
        private const int WM_HOTKEY = 0x0312;
        private readonly HotkeyWindow _window;
        private readonly int _legacyHotkeyId = 1001;
        private int _nextId = 2000;
        private readonly Dictionary<int, Action> _hotkeyCallbacks = new Dictionary<int, Action>();

        public bool IsRegistered { get; private set; }
        public event Action HotkeyPressed;

        public HotkeyService()
        {
            _window = new HotkeyWindow();
            _window.HotkeyReceived += OnHotkeyReceived;
        }

        private void OnHotkeyReceived(int id)
        {
            if (id == _legacyHotkeyId)
            {
                HotkeyPressed?.Invoke();
            }
            else if (_hotkeyCallbacks.TryGetValue(id, out var callback))
            {
                callback?.Invoke();
            }
        }

        /// <summary>
        /// Registers the legacy global hotkey (single hotkey mode, backward compatible).
        /// </summary>
        public bool Register(Keys modifiers, Keys key)
        {
            Unregister();

            uint modFlags = BuildModFlags(modifiers);
            IsRegistered = RegisterHotKey(_window.Handle, _legacyHotkeyId, modFlags, (uint)key);
            return IsRegistered;
        }

        /// <summary>
        /// Registers a new hotkey with a specific callback. Returns the hotkey id for later unregistration.
        /// </summary>
        public int Register(Keys modifiers, Keys key, Action callback)
        {
            int id = _nextId++;
            uint modFlags = BuildModFlags(modifiers);

            if (RegisterHotKey(_window.Handle, id, modFlags, (uint)key))
            {
                _hotkeyCallbacks[id] = callback;
                return id;
            }

            // Get last error for debugging
            int error = Marshal.GetLastWin32Error();
            string errorMsg = error == 1409 ? "Hotkey already registered by another application" : $"Error code: {error}";
            
            System.Diagnostics.Debug.WriteLine($"RegisterHotKey failed for {modifiers}+{key}. {errorMsg}");
            
            return -1;
        }
        
        /// <summary>
        /// Gets the last Win32 error message
        /// </summary>
        private string GetLastErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 1409: return "Hotkey already registered";
                case 5: return "Access denied";
                case 6: return "Invalid handle";
                case 87: return "Invalid parameter";
                default: return $"Error {errorCode}";
            }
        }

        /// <summary>
        /// Unregisters a specific hotkey by its id.
        /// </summary>
        public void Unregister(int id)
        {
            if (_window.Handle != IntPtr.Zero)
            {
                UnregisterHotKey(_window.Handle, id);
                _hotkeyCallbacks.Remove(id);
            }
        }

        /// <summary>
        /// Unregisters the legacy hotkey.
        /// </summary>
        public void Unregister()
        {
            if (IsRegistered && _window.Handle != IntPtr.Zero)
            {
                UnregisterHotKey(_window.Handle, _legacyHotkeyId);
                IsRegistered = false;
            }
        }

        private static uint BuildModFlags(Keys modifiers)
        {
            uint modFlags = 0;
            if (modifiers.HasFlag(Keys.Control)) modFlags |= 0x0002; // MOD_CONTROL
            if (modifiers.HasFlag(Keys.Alt)) modFlags |= 0x0001;     // MOD_ALT
            if (modifiers.HasFlag(Keys.Shift)) modFlags |= 0x0004;   // MOD_SHIFT
            if (modifiers.HasFlag(Keys.LWin) || modifiers.HasFlag(Keys.RWin)) modFlags |= 0x0008; // MOD_WIN
            return modFlags;
        }

        public void Dispose()
        {
            Unregister();
            foreach (var id in new List<int>(_hotkeyCallbacks.Keys))
                Unregister(id);
            _window.Dispose();
        }

        /// <summary>
        /// A hidden window implementation to listen for system-wide window messages (WndProc).
        /// </summary>
        private class HotkeyWindow : NativeWindow, IDisposable
        {
            public event Action<int> HotkeyReceived;

            public HotkeyWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                    HotkeyReceived?.Invoke((int)m.WParam);

                base.WndProc(ref m);
            }

            public void Dispose() => DestroyHandle();
        }
    }
}
