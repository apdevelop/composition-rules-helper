﻿using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ScreenGrid.Models.AppsInterop
{
    /// <summary>
    /// Native window wrapper
    /// </summary>
    public class NativeWindow
    {
        protected readonly IntPtr hWnd;

        private readonly int processId;

        public NativeWindow(IntPtr hWnd, int processId)
        {
            this.hWnd = hWnd;
            this.processId = processId;
        }

        public IntPtr Handle => this.hWnd;

        public int ProcessId => this.processId;

        public bool IsMinimized => WinApiInterop.NativeMethods.IsMinimized(this.hWnd);

        public bool IsVisible => WinApiInterop.NativeMethods.IsVisible(this.hWnd);

        public string ClassName
        {
            get
            {
                var lpClassName = new StringBuilder(1000);
                WinApiInterop.NativeMethods.GetClassName(this.hWnd, lpClassName, 1000);
                return lpClassName.ToString();
            }
        }

        public string Title => WinApiInterop.NativeMethods.GetWindowText(this.hWnd);

        public Point Location
        {
            get
            {
                var rect = new WinApiInterop.RECT();
                var b = WinApiInterop.NativeMethods.GetWindowRect(hWnd, ref rect);
                if (!b)
                {
                    ;
                }

                return new Point(rect.Left, rect.Top);
            }
        }

        public Rectangle Rect
        {
            get
            {
                var rect = new WinApiInterop.RECT();
                var b = WinApiInterop.NativeMethods.GetWindowRect(hWnd, ref rect);
                if (!b)
                {
                    ;
                }

                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
        }

        public Bitmap GetShot()
        {
            var rect = new WinApiInterop.RECT();
            var b = WinApiInterop.NativeMethods.GetWindowRect(this.hWnd, ref rect);
            return WinApiInterop.NativeMethods.GetWindowImage(this.hWnd, 0, 0, (rect.Right - rect.Left) + 1, (rect.Bottom - rect.Top) + 1); // TODO: +1 ?
        }

        public override string ToString() => this.ClassName;

        #region Utility methods

        public static List<NativeWindow> GetWindowsInTopMostOrder()
        {
            // Is there are win32 function to get a list off all top level windows
            // with a given class name?
            // Right now I am using EnumWindows (getting all windows) and then using
            // GetClassName on each window to see if it is a window that I care
            // about.

            // https://code.msdn.microsoft.com/windowsapps/Enumerate-top-level-9aa9d7c1
            // http://stackoverflow.com/a/296014

            var ClassNamesWhiteList = new[] { PhotoViewerWindow.MainWindowClassName, "IEFrame", "Chrome_WidgetWin_1", "MozillaWindowClass", "TLister", };
            var ClassNamesBlackList = new[] { "Shell_TrayWnd", "Button", "Alternate Owner", };
            var ClassNamesPartialBlackList = new[] { "ScreenGrid", "HwndWrapper", };

            var results = new List<NativeWindow>();

            WinApiInterop.NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                uint windowProcessId = 0;
                WinApiInterop.NativeMethods.GetWindowThreadProcessId(hWnd, out windowProcessId);

                var window = new NativeWindow(hWnd, (int)windowProcessId);

                // TODO: better checks for inappropriate windows
                // TODO: test in another environments

                // Skip inappropriate windows
                if ((!window.IsMinimized) && (window.IsVisible))
                {
                    var className = window.ClassName;

                    if (ClassNamesWhiteList.Any(s => String.Compare(s, className, StringComparison.Ordinal) == 0))
                    {
                        results.Add(window);
                        return true;
                    }
                    else if (ClassNamesBlackList.Any(s => String.Compare(s, className, StringComparison.Ordinal) == 0) ||
                             ClassNamesPartialBlackList.Any(s => className.Contains(s)))
                    {
                        return true;
                    }

                    results.Add(window);
                }

                return true;
            }, IntPtr.Zero);

            return results;
        }

        public static List<NativeWindow> GetMainWindowWindowOfProcesses(string processName)
        {
            return System.Diagnostics.Process.GetProcesses()
                .Where(p => String.Compare(p.ProcessName, processName, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(p => new NativeWindow(p.MainWindowHandle, p.Id))
                .ToList();
        }

        #endregion 
    }
}
