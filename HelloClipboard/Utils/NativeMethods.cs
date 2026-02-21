using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace HelloClipboard.Utils
{
    public static class NativeMethods
    {
        private const byte VK_CONTROL = 0x11;
        private const byte VK_V = 0x56;
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        public static bool IsCurrentProcessFocused()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero) return false;

            GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
            return foregroundProcessId == (uint)Process.GetCurrentProcess().Id;
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void SendCtrlV()
        {
            // Press Ctrl
            keybd_event(VK_CONTROL, 0, 0, 0);
            Thread.Sleep(10);
            
            // Press V
            keybd_event(VK_V, 0, 0, 0);
            Thread.Sleep(10);
            
            // Release V
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(10);
            
            // Release Ctrl
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// Sends Ctrl+V to the target window with proper focus handling
        /// </summary>
        public static void SendPasteToTarget(IntPtr targetHwnd)
        {
            if (targetHwnd == IntPtr.Zero || !IsWindow(targetHwnd))
            {
                SendPaste();
                return;
            }

            SetForegroundWindow(targetHwnd);
            Thread.Sleep(50);
            SendPaste();
        }

        /// <summary>
        /// Sends Ctrl+V using reliable input simulation
        /// </summary>
        public static void SendPaste()
        {
            Thread.Sleep(30);
            SendCtrlV();
        }
    }
}
