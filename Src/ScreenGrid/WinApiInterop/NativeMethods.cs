namespace ScreenGrid.WinApiInterop
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam); 

    public static class NativeMethods
    {
        private const int GWL_STYLE = (-16);

        public static bool IsMinimized(IntPtr hWnd)
        {
            int style = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);
            var minimized = ((style & (UInt32)WindowStyle.WS_MINIMIZE) == (UInt32)WindowStyle.WS_MINIMIZE);

            return minimized;
        }

        public static bool IsVisible(IntPtr hWnd)
        {
            int style = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);
            var visible = ((style & (UInt32)WindowStyle.WS_VISIBLE) == (UInt32)WindowStyle.WS_VISIBLE);

            return visible;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern UInt64 BitBlt
            (IntPtr hDestDC,
            int x, int y, int nWidth, int nHeight,
            IntPtr hSrcDC,
            int xSrc, int ySrc,
            System.Int32 dwRop);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindow", SetLastError = true)]
        public static extern IntPtr GetNextWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        public static string GetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            var length = GetWindowTextLength(hWnd);
            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static Bitmap GetWindowImage(IntPtr hWnd, int x, int y, int width, int height)
        {
            var image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(image))
            {
                var dc1 = graphics.GetHdc();
                var dc2 = GetWindowDC(hWnd);
                BitBlt(dc1, 0, 0, width, height, dc2, x, y, 13369376);
                graphics.ReleaseHdc(dc1);
            }

            return image;
        }

        public static void TraverseWindowTree(IntPtr hTmp, string windowClassName, ref bool found, ref IntPtr h)
        {
            if (found)
            {
                return;
            }

            if (hTmp == IntPtr.Zero)
            {
                found = false;
                return;
            }

            var lpClassName = new StringBuilder();
            GetClassName(hTmp, lpClassName, 100);
            var s = lpClassName.ToString();

            if (String.Compare(s, windowClassName) == 0)
            {
                found = true;
                h = hTmp;
                return;
            }
            else
            {
                var hChild = GetWindow(hTmp, (uint)GetWindowCommand.GW_CHILD);
                TraverseWindowTree(hChild, windowClassName, ref found, ref h);

                var hNext = GetWindow(hTmp, (uint)GetWindowCommand.GW_HWNDNEXT);
                TraverseWindowTree(hNext, windowClassName, ref found, ref h);
            }
        }
    }
}
