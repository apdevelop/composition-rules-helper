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

        public const double HeaderHeight = 24.0;

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

            // TODO: place calculated results in cache
            var isRotated = ((this.Rot == Rotation.R90) || (this.Rot == Rotation.R270));
            var gridLines = GridCreator.CreateGrid(this.gridMode, this.RenderWidth, this.RenderHeight, isRotated);

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

        private string AspectRatio
        {
            get
            {
                var ratio = ((double)this.ImageSize.Item1) / ((double)this.ImageSize.Item2);
                return Models.AspectRatioDetector.DetectCommonAspectRatio(ratio);
            }
        }

        public string CaptionText
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture,
                    "{0}\u00D7{1}{2}",
                    ImageSize.Item1,
                    ImageSize.Item2,
                    String.IsNullOrEmpty(this.AspectRatio) ? String.Empty : " (" + this.AspectRatio + ")");
            }
        }

        public void SnapToImageBounds()
        {
            // select foreground window from several processes of supported applications
            var nativeWindows = Models.AppsInterop.NativeWindow.GetWindowsInTopMostOrder();

            if (nativeWindows.Count > 0) // TODO: display error if no window was found
            {
                if (String.Compare(nativeWindows[0].ClassName, Models.AppsInterop.PhotoViewerWindow.MainWindowClassName, StringComparison.Ordinal) == 0)
                {
                    var photoViewerWindow = new Models.AppsInterop.PhotoViewerWindow(nativeWindows[0].Handle);

                    var rectViewedImage = photoViewerWindow.PhotoCanvasRect();
                    if (!rectViewedImage.IsEmpty)
                    {
                        var location = new GridTargetLocation { ImageBounds = rectViewedImage, Offset = Models.Geometry.Point.Zero, };
                        this.PositionWindow(location);
                    }
                }
                else
                {
                    var nativeWindow = nativeWindows[0];

                    // TODO: simplify Octane Render specific code
                    // Fixing issue with detached panels of Octane Render Standalone
                    if (Models.AppsInterop.OctaneRenderWindow.GetFromAllProcesses().Count > 0)
                    {
                        var octaneRenderWindows = nativeWindows
                            .Where(w => w.ClassName == Models.AppsInterop.OctaneRenderWindow.GetFromAllProcesses()[0].ClassName)
                            .OrderByDescending(w => w.Rect.Width)
                            .First();

                        // Select window with max size from topmost list
                        for (var i = 0; i < nativeWindows.Count; i++)
                        {
                            if (nativeWindows[i].ClassName != Models.AppsInterop.OctaneRenderWindow.GetFromAllProcesses()[0].ClassName)
                            {
                                break;
                            }
                            else
                            {
                                if (nativeWindows[i].Rect.Width == octaneRenderWindows.Rect.Width)
                                {
                                    nativeWindow = nativeWindows[i];
                                    break;
                                }
                            }
                        }
                    }

                    var bitmap = nativeWindow.GetShot();

                    Task.Factory.StartNew<GridTargetLocation>(() =>
                    {
                        var flatImage = new Models.FlatImage(bitmap);

                        Models.Geometry.Rectangle imageBounds;
                        if (Models.AppsInterop.OctaneRenderWindow.GetFromAllProcesses().Any(w => w.ClassName == nativeWindow.ClassName))
                        {
                            imageBounds = Models.AppsInterop.OctaneRenderWindow.FindRenderedImageBorders(flatImage);
                        }
                        else
                        {
                            imageBounds = flatImage.FindBoundsOfInnerImage();
                        }

                        var nativeWindowLocation = new Models.Geometry.Point(nativeWindow.Location.X, nativeWindow.Location.Y);
                        return new GridTargetLocation { ImageBounds = imageBounds, Offset = nativeWindowLocation, };
                    }).ContinueWith((t) =>
                    {
                        if (!t.Result.ImageBounds.IsEmpty)
                        {
                            if ((t.Result.ImageBounds.Width > 150) && (t.Result.ImageBounds.Height > 50))
                            {
                                this.PositionWindow(t.Result);
                            }
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void PositionWindow(GridTargetLocation location)
        {
            var diffX = 0.0;
            var diffY = HeaderHeight;
            // TODO: remove magiс numbers
            this.WindowLeft = location.ImageBounds.Left + location.Offset.X - diffX - 1;
            this.WindowTop = location.ImageBounds.Top + location.Offset.Y - diffY - 1;
            this.WindowWidth = (location.ImageBounds.Right - location.ImageBounds.Left) + diffX + 2;
            this.WindowHeight = (location.ImageBounds.Bottom - location.ImageBounds.Top) + diffY + 2;
        }
    }

    class GridTargetLocation
    {
        public Models.Geometry.Point Offset { get; set; }

        public Models.Geometry.Rectangle ImageBounds { get; set; }
    }
}
