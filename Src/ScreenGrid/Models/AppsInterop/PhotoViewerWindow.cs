using System;
using ScreenGrid.Models.Geometry;

namespace ScreenGrid.Models.AppsInterop
{
    public class PhotoViewerWindow : NativeWindow
    {
        public const string MainWindowClassName = "Photo_Lightweight_Viewer";
      
        private const string PhotoCanvasClassName = "Photos_PhotoCanvas";

        public PhotoViewerWindow(IntPtr hWnd, int processId)
            : base(hWnd, processId)
        {

        }

        public Rectangle PhotoCanvasRect()
        {
            bool found = false;
            IntPtr inner = IntPtr.Zero;
            WinApiInterop.NativeMethods.TraverseWindowTree(base.hWnd, PhotoViewerWindow.PhotoCanvasClassName, ref found, ref inner);

            if (found)
            {
                // Canvas
                var windowRectangle = new WinApiInterop.RECT();
                WinApiInterop.NativeMethods.GetWindowRect(inner, ref windowRectangle);

                var image = WinApiInterop.NativeMethods.GetWindowImage(inner, 0, 0, (windowRectangle.Right - windowRectangle.Left), (windowRectangle.Bottom - windowRectangle.Top));
                var flatImage = new FlatImage(image);

                var borders = PhotoViewerWindow.ImageBorders(flatImage);

                return new Rectangle
                {
                    X = windowRectangle.Left + borders.Left,
                    Y = windowRectangle.Top + borders.Top,
                    Width = borders.Width,
                    Height = borders.Height,
                };
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        private static Rectangle ImageBorders(FlatImage image)
        {
            var whiteColor = BitConverter.ToUInt32(new byte[] { 0, 255, 255, 255 }, 0);

            int? top = null;
            for (var iy = 0; iy < image.Height; iy++)
            {
                for (var ix = 0; ix < image.Width; ix++)
                {
                    if (image.pixels[ix, iy] != whiteColor)
                    {
                        top = iy;
                        break;
                    }
                }

                if (top.HasValue)
                {
                    break;
                }
            }

            int? bottom = null;
            for (var iy = image.Height - 1; iy >= 0; iy--)
            {
                for (var ix = 0; ix < image.Width; ix++)
                {
                    if (image.pixels[ix, iy] != whiteColor)
                    {
                        bottom = iy;
                        break;
                    }
                }

                if (bottom.HasValue)
                {
                    break;
                }
            }

            int? left = null;
            for (var ix = 0; ix < image.Width; ix++)
            {
                for (var iy = 0; iy < image.Height; iy++)
                {
                    if (image.pixels[ix, iy] != whiteColor)
                    {
                        left = ix;
                        break;
                    }
                }

                if (left.HasValue)
                {
                    break;
                }
            }

            int? right = null;
            for (var ix = image.Width - 1; ix >= 0; ix--)
            {
                for (var iy = 0; iy < image.Height; iy++)
                {
                    if (image.pixels[ix, iy] != whiteColor)
                    {
                        right = ix;
                        break;
                    }
                }

                if (right.HasValue)
                {
                    break;
                }
            }

            return new Rectangle
            { 
                X = left.Value, 
                Y = top.Value, 
                Width = right.Value - left.Value + 1, 
                Height = bottom.Value - top.Value + 1,
            };
        }
    }
}
