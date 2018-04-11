using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace PropertyDataGrid.Utilities
{
    public static class WindowsUtilities
    {
        public const int ApplicationIcon = 32512;

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern int GetGUIThreadInfo(int threadId, ref GUITHREADINFO info);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, int resourceId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hmonitor, ref MONITORINFO info);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT rect, int cPoints);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public int Width => right - left;
            public int Height => bottom - top;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const int GWL_STYLE = -16;
        private const int GW_OWNER = 4;

        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_MINIMIZE = 0x20000000;

        private const int MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;

        public static bool AttachThreadInput(int idAttach, int idAttachTo) => AttachThreadInput(idAttach, idAttachTo, true);
        public static bool DetachThreadInput(int idAttach, int idAttachTo) => AttachThreadInput(idAttach, idAttachTo, false);
        public static int GetWindowThreadId(IntPtr handle) => GetWindowThreadProcessId(handle, out int processId);

        public static int GetWindowProcessId(IntPtr handle)
        {
            GetWindowThreadProcessId(handle, out int processId);
            return processId;
        }

        public static IntPtr GetThreadActiveWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndActive;
        }

        public static IntPtr GetThreadCaptureWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndCapture;
        }

        public static IntPtr GetThreadCaretWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndCaret;
        }

        public static IntPtr GetThreadFocusWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndFocus;
        }

        public static IntPtr GetThreadMenuOwnerWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndMenuOwner;
        }

        public static IntPtr GetThreadMoveSizeWindow(int threadId)
        {
            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetGUIThreadInfo(threadId, ref info);
            return info.hwndMoveSize;
        }

        public static string GetWindowText(IntPtr handle)
        {
            int len = GetWindowTextLength(handle);
            var sb = new StringBuilder(len + 2);
            GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetWindowClass(IntPtr handle)
        {
            var sb = new StringBuilder(260);
            GetClassName(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        public static bool SetForegroundWindow(this Window window)
        {
            if (window == null)
                return false;

            var helper = new WindowInteropHelper(window);
            return SetForegroundWindow(helper.Handle);
        }

        public static IntPtr ActivateWindow(this Window window)
        {
            if (window == null)
                return IntPtr.Zero;

            var helper = new WindowInteropHelper(window);
            return ActivateWindow(helper.Handle);
        }

        public static void Center(this Window window) => Center(window, IntPtr.Zero);
        public static void Center(this Window window, IntPtr alternateOwner)
        {
            if (window == null)
                return;

            var helper = new WindowInteropHelper(window);
            CenterWindow(helper.Handle, alternateOwner);
        }

        public static void CenterWindow(IntPtr handle) => CenterWindow(handle, IntPtr.Zero);
        public static void CenterWindow(IntPtr handle, IntPtr alternateOwner)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException(null, nameof(handle));

            // determine owner window to center against
            int dwStyle = GetWindowLong(handle, GWL_STYLE);
            IntPtr hWndCenter = alternateOwner;
            if (alternateOwner == IntPtr.Zero)
            {
                if ((dwStyle & WS_CHILD) == WS_CHILD)
                {
                    hWndCenter = GetParent(handle);
                }
                else
                {
                    hWndCenter = GetWindow(handle, GW_OWNER);
                }
            }

            // get coordinates of the window relative to its parent
            var rcDlg = new RECT();
            GetWindowRect(handle, ref rcDlg);
            var rcArea = new RECT();
            var rcCenter = new RECT();
            if ((dwStyle & WS_CHILD) != WS_CHILD)
            {
                // don't center against invisible or minimized windows
                if (hWndCenter != IntPtr.Zero)
                {
                    int dwAlternateStyle = GetWindowLong(hWndCenter, GWL_STYLE);
                    if ((dwAlternateStyle & WS_VISIBLE) != WS_VISIBLE || (dwAlternateStyle & WS_MINIMIZE) == WS_VISIBLE)
                    {
                        hWndCenter = IntPtr.Zero;
                    }
                }

                var mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

                // center within appropriate monitor coordinates
                if (hWndCenter == IntPtr.Zero)
                {
                    IntPtr hwDefault = GetActiveWindow();
                    GetMonitorInfo(MonitorFromWindow(hwDefault, MONITOR_DEFAULTTOPRIMARY), ref mi);
                    rcCenter = mi.rcWork;
                    rcArea = mi.rcWork;
                }
                else
                {
                    GetWindowRect(hWndCenter, ref rcCenter);
                    GetMonitorInfo(MonitorFromWindow(hWndCenter, MONITOR_DEFAULTTONEAREST), ref mi);
                    rcArea = mi.rcWork;
                }
            }
            else
            {
                // center within parent client coordinates
                IntPtr hWndParent = GetParent(handle);
                GetClientRect(hWndParent, ref rcArea);
                GetClientRect(hWndCenter, ref rcCenter);
                MapWindowPoints(hWndCenter, hWndParent, ref rcCenter, 2);
            }

            // find dialog's upper left based on rcCenter
            int xLeft = (rcCenter.left + rcCenter.right) / 2 - rcDlg.Width / 2;
            int yTop = (rcCenter.top + rcCenter.bottom) / 2 - rcDlg.Height / 2;

            // if the dialog is outside the screen, move it inside
            if (xLeft + rcDlg.Width > rcArea.right)
            {
                xLeft = rcArea.right - rcDlg.Width;
            }

            if (xLeft < rcArea.left)
            {
                xLeft = rcArea.left;
            }

            if (yTop + rcDlg.Height > rcArea.bottom)
            {
                yTop = rcArea.bottom - rcDlg.Height;
            }

            if (yTop < rcArea.top)
            {
                yTop = rcArea.top;
            }

            // map screen coordinates to child coordinates
            SetWindowPos(handle, IntPtr.Zero, xLeft, yTop, -1, -1, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
        }

        public static IntPtr ActivateModalWindow(IntPtr hwnd) => ActivateWindow(GetModalWindow(hwnd));
        public static IntPtr ActivateWindow(IntPtr hwnd) => ModalWindowUtil.ActivateWindow(hwnd);
        public static IntPtr GetModalWindow(IntPtr owner) => ModalWindowUtil.GetModalWindow(owner);

        private class ModalWindowUtil
        {
            private int _maxOwnershipLevel;
            private IntPtr _maxOwnershipHandle;

            private delegate bool EnumChildrenCallback(IntPtr hwnd, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool EnumThreadWindows(int dwThreadId, EnumChildrenCallback lpEnumFunc, IntPtr lParam);

            private bool EnumChildren(IntPtr hwnd, IntPtr lParam)
            {
                int level = 1;
                if (IsWindowVisible(hwnd) && IsOwned(lParam, hwnd, ref level))
                {
                    if (level > _maxOwnershipLevel)
                    {
                        _maxOwnershipHandle = hwnd;
                        _maxOwnershipLevel = level;
                    }
                }
                return true;
            }

            private static bool IsOwned(IntPtr owner, IntPtr hwnd, ref int level)
            {
                var ownerHandle = GetWindow(hwnd, GW_OWNER);
                if (ownerHandle == IntPtr.Zero)
                    return false;

                if (ownerHandle == owner)
                    return true;

                level++;
                return IsOwned(owner, ownerHandle, ref level);
            }

            public static IntPtr ActivateWindow(IntPtr hwnd)
            {
                if (hwnd == IntPtr.Zero)
                    return IntPtr.Zero;

                return SetActiveWindow(hwnd);
            }

            public static IntPtr GetModalWindow(IntPtr owner)
            {
                var util = new ModalWindowUtil();
                EnumThreadWindows(GetCurrentThreadId(), util.EnumChildren, owner);
                return util._maxOwnershipHandle; // may be IntPtr.Zero
            }
        }

        public static bool SetConsoleIcon(int resourceId)
        {
            try
            {
                const int ICON_SMALL = 0;
                const int ICON_BIG = 1;
                const int WM_SETICON = 0x80;

                IntPtr hwnd = GetConsoleWindow();
                IntPtr icon = (resourceId <= 0 || resourceId > ushort.MaxValue) ? IntPtr.Zero : LoadIcon(Process.GetCurrentProcess().MainModule.BaseAddress, resourceId);
                SendMessage(hwnd, WM_SETICON, new IntPtr(ICON_SMALL), icon);
                SendMessage(hwnd, WM_SETICON, new IntPtr(ICON_BIG), icon);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
