namespace ScreenGrid.Models.AppsInterop
{
    using System;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Native window wrapper of Octane Render
    /// </summary>
    public class OctaneRenderWindow : NativeWindow
    {
        public OctaneRenderWindow(IntPtr hWnd)
            : base(hWnd)
        {

        }

        public const string ProcessName = "octane";

        public static OctaneRenderWindow GetFromFirstProcess()
        {
            var process = System.Diagnostics.Process.GetProcesses()
                .Where(p => String.Compare(p.ProcessName, ProcessName, StringComparison.OrdinalIgnoreCase) == 0)
                .FirstOrDefault();

            return (process != null) ? new OctaneRenderWindow(process.MainWindowHandle) : null;
        }

        private static Bitmap UpperLeftCorner1
        {
            get
            {
                return ReadPngResouse("ScreenGrid.Models.Resources.UpperLeftCorner1.png");
            }
        }

        private static Bitmap UpperLeftCorner2
        {
            get
            {
                return ReadPngResouse("ScreenGrid.Models.Resources.UpperLeftCorner2.png");
            }
        }

        private static Bitmap UpperLeftCorner3
        {
            get
            {
                return ReadPngResouse("ScreenGrid.Models.Resources.UpperLeftCorner3.png");
            }
        }

        private static Bitmap LowerRightCorner1
        {
            get
            {
                return ReadPngResouse("ScreenGrid.Models.Resources.LowerRightCorner1.png");
            }
        }

        private static Bitmap LowerRightCorner2
        {
            get
            {
                return ReadPngResouse("ScreenGrid.Models.Resources.LowerRightCorner2.png");
            }
        }

        private static Bitmap ReadPngResouse(string resourceName)
        {
            var imageData = ReadResourceFile(resourceName);
            using (var ms = new System.IO.MemoryStream(imageData))
            {
                return new Bitmap(ms);
            }
        }

        private static Point TopLeftCorner(FlatImage image)
        {
            var topLeftCornerFragments = new[]
            {
                new FlatImage(OctaneRenderWindow.UpperLeftCorner1),
                new FlatImage(OctaneRenderWindow.UpperLeftCorner2),
                new FlatImage(OctaneRenderWindow.UpperLeftCorner3),
            };

            var topLeftCornerX = 0;
            var topLeftCornerY = 0;
            foreach (var topLeftCornerFragment in topLeftCornerFragments)
            {
                for (var x = 0; x < image.Width - topLeftCornerFragment.Width; x++)
                {
                    for (var y = 0; y < image.Height - topLeftCornerFragment.Height; y++)
                    {
                        if (image.CompareWithFragmentWithTolerance(topLeftCornerFragment, x, y))
                        {
                            topLeftCornerX = x + topLeftCornerFragment.Width - 1;
                            topLeftCornerY = y + topLeftCornerFragment.Height - 1;
                            return new Point(topLeftCornerX, topLeftCornerY);
                        }
                    }
                }
            }

            return new Point(0, 0);
        }

        private static Point BottomRightCorner(FlatImage image)
        {
            var bottomRightCornerFragments = new[]
            {
                new FlatImage(OctaneRenderWindow.LowerRightCorner1),
                new FlatImage(OctaneRenderWindow.LowerRightCorner2),
            };

            var bottomRightCornerX = 0;
            var bottomRightCornerY = 0;
            foreach (var bottomRightCornerFragment in bottomRightCornerFragments)
            {
                for (var x = image.Width - bottomRightCornerFragment.Width - 1; x > 0; x--)
                {
                    for (var y = 0; y < image.Height - bottomRightCornerFragment.Height; y++)
                    {
                        if (image.CompareWithFragmentWithTolerance(bottomRightCornerFragment, x, y))
                        {
                            bottomRightCornerX = x;
                            bottomRightCornerY = y;
                            return new Point(bottomRightCornerX, bottomRightCornerY);
                        }
                    }
                }
            }

            return new Point(0, 0);
        }

        /// <summary>
        /// Search image top left point
        /// </summary>
        /// <param name="image"></param>
        /// <param name="topLeftCorner"></param>
        /// <param name="bottomRightCorner"></param>
        /// <returns></returns>
        private static Point TopLeftImage(FlatImage image, Point topLeftCorner, Point bottomRightCorner)
        {
            var topLeftImageX = 0;
            var topLeftImageY = 0;

            var canvasColor = image.pixels[(int)topLeftCorner.X, (int)topLeftCorner.Y];

            for (var y = (int)topLeftCorner.Y; y < (int)bottomRightCorner.Y; y++)
            {
                for (var x = (int)topLeftCorner.X; x < (int)bottomRightCorner.X; x++)
                {
                    if (image.pixels[x, y] != canvasColor)
                    {
                        if (y > (int)topLeftCorner.Y)
                        {
                            topLeftImageX = x;
                            topLeftImageY = y;
                            return new Point(topLeftImageX, topLeftImageY);
                        }

                        break;
                    }
                }
            }

            return new Point(0, 0);
        }

        /// <summary>
        /// Search image bottom right point
        /// </summary>
        /// <param name="image"></param>
        /// <param name="topLeftCorner"></param>
        /// <param name="bottomRightCorner"></param>
        /// <returns></returns>
        private static Point BottomRightImage(FlatImage image, Point topLeftCorner, Point bottomRightCorner)
        {
            var areaColor2 = image.pixels[(int)bottomRightCorner.X, (int)bottomRightCorner.Y];

            for (var y = (int)bottomRightCorner.Y - 1; y > (int)topLeftCorner.Y; y--)
            {
                for (var x = (int)bottomRightCorner.X - 1; x > (int)topLeftCorner.X; x--)
                {
                    if (image.pixels[x, y] != areaColor2)
                    {
                        if (y < bottomRightCorner.Y)
                        {
                            return new Point(x, y);
                        }

                        break;
                    }
                }
            }

            return new Point(0, 0);
        }

        public static Geometry.Rectangle FindRenderedImageBorders(FlatImage flatImage)
        {
            var topLeftCorner = OctaneRenderWindow.TopLeftCorner(flatImage);
            var bottomRightCorner = OctaneRenderWindow.BottomRightCorner(flatImage);
            var topLeftImage = OctaneRenderWindow.TopLeftImage(flatImage, topLeftCorner, bottomRightCorner);
            var bottomRightImage = OctaneRenderWindow.BottomRightImage(flatImage, topLeftCorner, bottomRightCorner);

            // TODO: && topLeftImageFound && bottomRightImageFound)
            if ((topLeftCorner.X > 0) && (topLeftCorner.Y > 0) && (bottomRightCorner.X > 0) && (bottomRightCorner.Y > 0) && (topLeftImage.X > 0))
            {
                return new Geometry.Rectangle
                {
                    X = topLeftImage.X,
                    Y = topLeftImage.Y,
                    Width = bottomRightImage.X - topLeftImage.X + 1,
                    Height = bottomRightImage.Y - topLeftImage.Y + 1,
                };
            }
            else
            {
                return Geometry.Rectangle.Empty;
            }
        }

        private static byte[] ReadResourceFile(string id)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            byte[] data = null;
            using (var stream = assembly.GetManifestResourceStream(id))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
            }

            return data;
        }
    }
}
