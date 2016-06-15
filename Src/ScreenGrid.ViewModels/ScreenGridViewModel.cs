namespace ScreenGrid.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using ScreenGrid.Models.Grids;
    using ScreenGrid.ViewModels.Utils;

    public class ScreenGridViewModel : BaseNotifyPropertyChanged
    {
        private Grid contentControl;

        public ScreenGridViewModel()
        {
            this.contentControl = new Grid
            {
                IsHitTestVisible = false,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            this.RotateClockwiseCommand = new RelayCommand((o) => { this.Rot = Rotator.RotateClockwise(this.Rot); });
            this.RotateCounterClockwiseCommand = new RelayCommand((o) => { this.Rot = Rotator.RotateCounterClockwise(this.Rot); });
            this.FlipHorizontalCommand = new RelayCommand((o) => { this.FlipH = !this.FlipH; });
            this.FlipVerticalCommand = new RelayCommand((o) => { this.FlipV = !this.FlipV; });
            this.SnapCommand = new RelayCommand((o) => { this.SnapToImageBounds(); });
        }

        public ICommand RotateClockwiseCommand { get; private set; }
        public ICommand RotateCounterClockwiseCommand { get; private set; }
        public ICommand FlipHorizontalCommand { get; private set; }
        public ICommand FlipVerticalCommand { get; private set; }
        public ICommand SnapCommand { get; private set; }

        private GridType gridMode;
        public GridType GridMode
        {
            set
            {
                if (this.gridMode != value)
                {
                    this.gridMode = value;
                    this.UpdateContentControl();
                }
            }
        }

        private Rotation rotation = Rotation.R0;
        private bool isFlippedHorizontal = false;
        private bool isFlippedVertical = false;

        private Rotation Rot
        {
            get
            {
                return this.rotation;
            }

            set
            {
                if (this.rotation != value)
                {
                    this.rotation = value;
                    this.UpdateContentControl();
                }
            }
        }

        private bool FlipH
        {
            get
            {
                return this.isFlippedHorizontal;
            }

            set
            {
                if (this.isFlippedHorizontal != value)
                {
                    this.isFlippedHorizontal = value;
                    this.UpdateContentControl();
                }
            }
        }

        private bool FlipV
        {
            get
            {
                return this.isFlippedVertical;
            }

            set
            {
                if (this.isFlippedVertical != value)
                {
                    this.isFlippedVertical = value;
                    this.UpdateContentControl();
                }
            }
        }

        private Color selectedLineColor = Colors.White;

        private static readonly List<Color> lineColors = new List<Color>(new[]
                {
                    Colors.White,
                    Colors.Magenta,
                    Colors.Black,
                });

        public IList<ColorItemViewModel> LineColors
        {
            get
            {
                return lineColors.Select(c => new ColorItemViewModel(c)).ToList();
            }
        }

        public ColorItemViewModel SelectedLineColor
        {
            get
            {
                return new ColorItemViewModel(this.selectedLineColor);
            }

            set
            {
                if (this.selectedLineColor != value.Color)
                {
                    this.selectedLineColor = value.Color;
                    this.UpdateContentControl();
                }
            }
        }

        private Brush LineBrush
        {
            get
            {
                return new SolidColorBrush(this.selectedLineColor) { Opacity = 0.75 };
            }
        }

        private double RenderWidth
        {
            get
            {
                return this.contentControl.RenderSize.Width;
            }
        }

        private double RenderHeight
        {
            get
            {
                return this.contentControl.RenderSize.Height;
            }
        }

        public Grid ContentControl
        {
            get
            {
                return this.contentControl;
            }
        }

        #region Window position and size

        private const double OuterBorderWidth = 1.0;
        private const double HeaderHeight = 24.0;

        private double windowWidth = 400.0 + 2 * OuterBorderWidth;
        public double WindowWidth
        {
            get
            {
                return this.windowWidth;
            }

            set
            {
                if (this.windowWidth != value)
                {
                    this.windowWidth = value;
                    base.OnPropertyChanged("WindowWidth");
                    UpdateContentControl(); // base.OnPropertyChanged("CaptionText");
                }
            }
        }

        private double windowHeight = 300.0 + 2 * OuterBorderWidth + HeaderHeight;
        public double WindowHeight
        {
            get
            {
                return this.windowHeight;
            }

            set
            {
                if (this.windowHeight != value)
                {
                    this.windowHeight = value;
                    base.OnPropertyChanged("WindowHeight");
                    UpdateContentControl(); // base.OnPropertyChanged("CaptionText");
                }
            }
        }

        private double windowLeft = 400.0;
        public double WindowLeft
        {
            get
            {
                return this.windowLeft;
            }

            set
            {
                if (this.windowLeft != value)
                {
                    this.windowLeft = value;
                    base.OnPropertyChanged("WindowLeft");
                }
            }
        }

        private double windowTop = 400.0;
        public double WindowTop
        {
            get
            {
                return this.windowTop;
            }

            set
            {
                if (this.windowTop != value)
                {
                    this.windowTop = value;
                    base.OnPropertyChanged("WindowTop");
                }
            }
        }

        #endregion

        public void UpdateContentControl()
        {
            this.contentControl.Children.Clear();

            var gridLines = GridCreator.CreateGrid(this.gridMode, this.RenderWidth, this.RenderHeight);

            var lines = GridCreator.Transform(gridLines, this.rotation, this.isFlippedHorizontal, this.isFlippedVertical, this.RenderWidth, this.RenderHeight);
            for (var i = 0; i < lines.Length; i++)
            {
                this.contentControl.Children.Add(CreateLine(lines[i].p1.X, lines[i].p1.Y, lines[i].p2.X, lines[i].p2.Y));
            }

            base.OnPropertyChanged("CaptionText");
        }

        private System.Windows.Shapes.Line CreateLine(double x1, double y1, double x2, double y2)
        {
            return new System.Windows.Shapes.Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                Stroke = this.LineBrush,
                IsHitTestVisible = false,
                SnapsToDevicePixels = false,
            };
        }

        private Tuple<int, int> ImageSize
        {
            get
            {
                return new Tuple<int, int>(
                    (int)(this.WindowWidth - 2 * OuterBorderWidth),
                    (int)(this.WindowHeight - 2 * OuterBorderWidth - HeaderHeight));
            }
        }

        // TODO: move code to Models, add tests

        /// <summary>
        /// Common aspect ratios
        /// https://en.wikipedia.org/wiki/Aspect_ratio_%28image%29
        /// </summary>
        private static Dictionary<double, string> CommonAspectRatios = new Dictionary<double, string>()
        {
            // TODO: create class with separated properties
            // TODO: add descriptions for displaying in tooltip
            { 1.0, "1:1" },
            { 6.0 / 5.0, "6:5" },
            { 5.0 / 4.0, "5:4" },
            { 4.0 / 3.0, "4:3" },
            { 11.0 / 8.0, "11:8" },
            { Math.Sqrt(2.0), "1.41:1" }, // ISO 216 paper sizes
            { 1.43, "1.43:1" },
            { 3.0 / 2.0, "3:2" },
            { 16.0 / 10.0, "16:10" }, // The golden ratio
            { 1.618, "16.18:10" },
            { 5.0 / 3.0, "5:3" },
            { 16.0 / 9.0, "16:9" },
            { 1.85, "1.85:1" },
            { 2.35, "2.35:1" },
            { 2.39, "2.39:1" },
            { 2.414, "2.414:1" }, // The silver ratio
            { 2.76, "2.76:1" },
        };

        private string AspectRatio
        {
            get
            {
                const double tolerance = 0.01;
                var ratio = ((double)this.ImageSize.Item1) / ((double)this.ImageSize.Item2);

                if (ratio >= 1.0)
                {
                    // Horizontal
                    var nearest = CommonAspectRatios.OrderBy(kvp => Math.Abs(kvp.Key - ratio)).First();
                    if (Math.Abs(nearest.Key - ratio) < tolerance)
                    {
                        return nearest.Value;
                    }
                }
                else
                {
                    // Vertical
                    ratio = 1.0 / ratio;
                    var nearest = CommonAspectRatios.OrderBy(kvp => Math.Abs(kvp.Key - ratio)).First();
                    if (Math.Abs(nearest.Key - ratio) < tolerance)
                    {
                        var arr = nearest.Value.Split(':');
                        return String.Format(CultureInfo.InvariantCulture, "{0}:{1}", arr[1], arr[0]);
                    }
                }

                return String.Empty;
            }
        }

        public string CaptionText
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture,
                    "{0}\u00D7{1}{2}",
                    ImageSize.Item1,
                    ImageSize.Item2,
                    String.IsNullOrEmpty(this.AspectRatio) ? String.Empty : " (" + this.AspectRatio + ")");
            }
        }

        public void SnapToImageBounds()
        {
            // select foreground window from several processes of supported applications
            var nativeWindow = Models.AppsInterop.NativeWindow.GetTopMostWindow();

            if (nativeWindow != null) // TODO: display error if no window was found
            {
                if (nativeWindow.ClassName == Models.AppsInterop.PhotoViewerWindow.MainWindowClassName)
                {
                    var photoViewerWindow = new Models.AppsInterop.PhotoViewerWindow(nativeWindow.Handle);

                    var rectViewedImage = photoViewerWindow.PhotoCanvasRect();
                    if (!rectViewedImage.IsEmpty)
                    {
                        this.PositionWindow(Models.Geometry.Point.Zero, rectViewedImage);
                    }
                }
                else
                {
                    Task.Factory.StartNew<Tuple<Models.Geometry.Rectangle, Models.Geometry.Point>>(() =>
                    {
                        var bitmap = nativeWindow.GetShot();
                        var flatImage = new Models.FlatImage(bitmap);

                        Models.Geometry.Rectangle imageBounds;
                        if (Models.AppsInterop.OctaneRenderWindow.GetFromAllProcesses().Any(w => w.ClassName == nativeWindow.ClassName))
                        {
                            // TODO: remove this Octane Render specific code
                            imageBounds = Models.AppsInterop.OctaneRenderWindow.FindRenderedImageBorders(flatImage);
                        }
                        else
                        {
                            imageBounds = flatImage.FindBoundsOfInnerImage();
                        }

                        var nativeWindowLocation = new Models.Geometry.Point(nativeWindow.Location.X, nativeWindow.Location.Y);
                        return new Tuple<Models.Geometry.Rectangle, Models.Geometry.Point>(imageBounds, nativeWindowLocation);
                    }).ContinueWith((t) =>
                    {
                        var imageBounds = t.Result.Item1;
                        var windowLocation = t.Result.Item2;
                        if (!imageBounds.IsEmpty)
                        {
                            if ((imageBounds.Width > 150) && (imageBounds.Height > 50))
                            {
                                this.PositionWindow(t.Result.Item2, t.Result.Item1);
                            }
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void PositionWindow(Models.Geometry.Point parentLocation, Models.Geometry.Rectangle rectRenderedImage)
        {
            var diffX = 0.0;
            var diffY = HeaderHeight;
            // TODO: remove magiс numbers
            this.WindowLeft = rectRenderedImage.Left + parentLocation.X - diffX - 1;
            this.WindowTop = rectRenderedImage.Top + parentLocation.Y - diffY - 1;
            this.WindowWidth = (rectRenderedImage.Right - rectRenderedImage.Left) + diffX + 2;
            this.WindowHeight = (rectRenderedImage.Bottom - rectRenderedImage.Top) + diffY + 2;
        }
    }
}
