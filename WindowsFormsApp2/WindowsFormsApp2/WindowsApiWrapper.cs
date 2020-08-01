using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsFormsApp2
{
    class WindowsApiWrapper
    {
        #region DLLS
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        #endregion

        #region CONSTANTS
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOCUS_CHANGE = 0x8005;
        #endregion

        private static IntPtr m_hhook;
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        
        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        public static void HookFocusChangeEvent(WinEventDelegate handler)
        {
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOCUS_CHANGE, EVENT_SYSTEM_FOCUS_CHANGE, IntPtr.Zero, handler, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static string Username { get; } = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    }
}
