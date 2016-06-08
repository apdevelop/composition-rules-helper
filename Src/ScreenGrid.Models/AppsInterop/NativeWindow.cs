﻿namespace ScreenGrid.Models.AppsInterop
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Collections.Generic;

    /// <summary>
    /// Native window wrapper
    /// </summary>
    public class NativeWindow
    {
        protected readonly IntPtr hWnd;

        public NativeWindow(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public IntPtr Handle
        {
            get
            {
                return this.hWnd;
            }
        }

        public bool IsMinimized
        {
            get
            {
                return WinApiInterop.NativeMethods.IsMinimized(this.hWnd);
            }
        }

        public bool IsVisible
        {
            get
            {
                return WinApiInterop.NativeMethods.IsVisible(this.hWnd);
            }
        }

        public string ClassName
        {
            get
            {
                var lpClassName = new StringBuilder(1000);
                WinApiInterop.NativeMethods.GetClassName(hWnd, lpClassName, 1000);

                return lpClassName.ToString();
            }
        }

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

        public static NativeWindow GetTopMostWindow(IEnumerable<string> classNames)
        {
            // Is there are win32 function to get a list off all top level windows
            // with a given class name?
            // Right now I am using EnumWindows (getting all windows) and then using
            // GetClassName on each window to see if it is a window that I care
            // about.

            // https://code.msdn.microsoft.com/windowsapps/Enumerate-top-level-9aa9d7c1

            var results = new List<IntPtr>();
            //WinApiInterop.WinApiUtils.EnumWindows(new WinApiInterop.WinApiUtils.EnumWindowsProc(EnumTheWindows), IntPtr.Zero);

            // http://stackoverflow.com/a/296014

            WinApiInterop.NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                var win = new NativeWindow(hWnd);

                foreach (var className in classNames)
                {
                    if ((String.Compare(win.ClassName, className, StringComparison.OrdinalIgnoreCase) == 0) &&
                        (!win.IsMinimized) && 
                        (win.IsVisible))
                    {
                        results.Add(hWnd);
                    }
                }

                return true;
            }, IntPtr.Zero);

            if (results.Count > 0)
            {
                return new NativeWindow(results[0]);
            }
            else
            {
                return null;
            }
        }
    }
}
