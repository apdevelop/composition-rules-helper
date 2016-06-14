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

        public string CaptionText
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture,
                    "{0}\u00D7{1}",
                    (int)(this.WindowWidth - 2 * OuterBorderWidth),
                    (int)(this.WindowHeight - 2 * OuterBorderWidth - HeaderHeight));
            }
        }

        public void SnapToImageBounds()
        {
            // select foreground window from several processes of supported applications
            var nativeWindow = Models.AppsInterop.NativeWindow.GetTopMostWindow();

            if (nativeWindow != null)
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
