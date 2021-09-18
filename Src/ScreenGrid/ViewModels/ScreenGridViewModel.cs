namespace ScreenGrid.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            this.SwitchGridSelectorCommand = new RelayCommand((o) => { this.IsGridSelectorVisible = !this.isGridSelectorVisible; });
            this.SelectGridCommand = new RelayCommand((o) =>
            {
                this.SelectedGridType = (GridType)o;
                this.IsGridSelectorVisible = false;
                // TODO: highlight button (set background color)
            });
        }

        public ICommand RotateClockwiseCommand { get; private set; }
        public ICommand RotateCounterClockwiseCommand { get; private set; }
        public ICommand FlipHorizontalCommand { get; private set; }
        public ICommand FlipVerticalCommand { get; private set; }
        public ICommand SnapCommand { get; private set; }
        public ICommand SwitchGridSelectorCommand { get; private set; }
        public ICommand SelectGridCommand { get; private set; }

        private GridType selectedGridType;

        /// <summary>
        /// Current selected grid type
        /// </summary>
        public GridType SelectedGridType
        {
            get
            {
                return this.selectedGridType;
            }

            set
            {
                if (this.selectedGridType != value)
                {
                    this.selectedGridType = value;
                    this.UpdateContentControl();
                    base.OnPropertyChanged(nameof(this.IsFlipEnabled));
                    base.OnPropertyChanged(nameof(this.IsRotateEnabled));
                }
            }
        }

        // TODO: implement full featured viewmodel for List<GridModeItem>

        public bool IsFlipEnabled
        {
            get
            {
                if (this.SelectedGridType == GridType.None)
                {
                    return false;
                }
                else
                {
                    return GridModeItem.List
                        .Single(g => g.GridType == this.SelectedGridType)
                        .IsFlipEnabled;
                }
            }
        }

        public bool IsRotateEnabled
        {
            get
            {
                if (this.SelectedGridType == GridType.None)
                {
                    return false;
                }
                else
                {
                    return GridModeItem.List
                        .Single(g => g.GridType == this.SelectedGridType)
                        .IsRotateEnabled;
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

        private bool isGridSelectorVisible = false;

        public bool IsGridSelectorVisible
        {
            get
            {
                return this.isGridSelectorVisible;
            }

            set
            {
                if (this.isGridSelectorVisible != value)
                {
                    this.isGridSelectorVisible = value;
                    base.OnPropertyChanged(nameof(GridSelectorVisibility));
                }
            }
        }

        public Visibility GridSelectorVisibility
        {
            get
            {
                return this.IsGridSelectorVisible ? Visibility.Visible : Visibility.Collapsed;
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
                    base.OnPropertyChanged(nameof(WindowWidth));
                    this.UpdateContentControl();
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
                    base.OnPropertyChanged(nameof(WindowHeight));
                    this.UpdateContentControl();
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
                    base.OnPropertyChanged(nameof(WindowLeft));
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
                    base.OnPropertyChanged(nameof(WindowTop));
                }
            }
        }

        #endregion

        #region Grips options

        public int GripLength { get => 50; }

        public int GripWidth { get => 4; }

        public int CornerGripSize { get => 6; }

        #endregion

        private void UpdateContentControl()
        {
            this.contentControl.Children.Clear();

            var isRotated = (this.Rot == Rotation.R90) || (this.Rot == Rotation.R270);
            var gridLines = GridCreator.CreateGrid(this.SelectedGridType, this.RenderWidth, this.RenderHeight, isRotated);

            var lines = GridCreator.Transform(gridLines, this.rotation, this.isFlippedHorizontal, this.isFlippedVertical, this.RenderWidth, this.RenderHeight);
            foreach (var line in lines)
            {
                this.contentControl.Children.Add(CreateLine(line.p1.X, line.p1.Y, line.p2.X, line.p2.Y));
            }

            // TODO: ? Dark overlay for clipped areas
            ////this.contentControl.Children.Add(new System.Windows.Shapes.Rectangle
            ////{
            ////    Width = 50,
            ////    Height = this.RenderHeight,
            ////    VerticalAlignment = VerticalAlignment.Center,
            ////    HorizontalAlignment = HorizontalAlignment.Left,
            ////    Stroke = this.LineBrush,
            ////    Fill = new SolidColorBrush(Color.FromArgb(127, 0, 0, 0)),
            ////    IsHitTestVisible = false,
            ////    SnapsToDevicePixels = false,
            ////});

            base.OnPropertyChanged(nameof(CaptionText));
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
            // Select topmost window from several processes of supported applications

            // Ordered list of windows, from top to bottom
            var nativeWindows = Models.AppsInterop.NativeWindow.GetWindowsInTopMostOrder()
                .Select(w => new Models.NativeWindowState
                {
                    Handle = w.Handle,
                    ClassName = w.ClassName,
                    Width = w.Rect.Width,
                    Height = w.Rect.Height,
                    Caption = w.Title,
                })
                .ToList();

            if (nativeWindows.Count > 0)
            {
                // TODO: refactor - move priority logic to model
                if (String.Compare(nativeWindows[0].ClassName, Models.AppsInterop.PhotoViewerWindow.MainWindowClassName, StringComparison.Ordinal) == 0)
                {
                    var photoViewerWindow = new Models.AppsInterop.PhotoViewerWindow(nativeWindows[0].Handle);

                    var rectViewedImage = photoViewerWindow.PhotoCanvasRect();
                    if (!rectViewedImage.IsEmpty)
                    {
                        var location = new Models.GridTargetLocation { ImageBounds = rectViewedImage, Offset = Models.Geometry.Point.Zero, };
                        this.SetWindowLocation(location);
                    }
                }
                else
                {
                    var processWindowsList = new List<Models.AppsInterop.NativeWindow>();

                    // TODO: dictionary <KnownApp, Windows> instead of separate lists
                    var octaneRenderWindows = Models.AppsInterop.NativeWindow.GetMainWindowWindowOfProcesses(Models.AppsInterop.OctaneRenderWindow.ProcessName);
                    processWindowsList.AddRange(octaneRenderWindows);

                    var paintNetWindows = Models.AppsInterop.NativeWindow.GetMainWindowWindowOfProcesses("PaintDotNet");
                    processWindowsList.AddRange(paintNetWindows);

                    var irfanViewWindows = Models.AppsInterop.NativeWindow.GetMainWindowWindowOfProcesses("i_view64");
                    processWindowsList.AddRange(irfanViewWindows);

                    // Add topmost window from any other application
                    if (!processWindowsList.Any(pw => pw.Handle == nativeWindows[0].Handle))
                    {
                        processWindowsList.Add(new Models.AppsInterop.NativeWindow(nativeWindows[0].Handle));
                    }

                    // TODO: ? main process window - check if it has max size ? search by window class
                    // SelectMainWindow(nativeWindows, topMostProcessWindow.ClassName);

                    var topmostWindow = processWindowsList
                        .OrderBy(pw => nativeWindows.FindIndex(nw => nw.Handle == pw.Handle))
                        .First();

                    var knownApp = KnownApp.Unknown;
                    if (octaneRenderWindows.Any(w => w.Handle == topmostWindow.Handle))
                    {
                        knownApp = KnownApp.OctaneRenderStandalone;
                    }
                    else if (paintNetWindows.Any(w => w.Handle == topmostWindow.Handle))
                    {
                        knownApp = KnownApp.PaintDotNet;
                    }
                    else if (irfanViewWindows.Any(w => w.Handle == topmostWindow.Handle))
                    {
                        knownApp = KnownApp.IrfanView;
                    }

                    Debug.WriteLine($"Topmost process window: {topmostWindow}  App: {knownApp}");

                    var selectedNativeWindow = new Models.AppsInterop.NativeWindow(topmostWindow.Handle);
                    var bitmap = selectedNativeWindow.GetShot();

                    Task.Factory.StartNew(() =>
                    {
                        var flatImage = new Models.FlatImage(bitmap);

                        Models.Geometry.Rectangle imageBounds;
                        switch (knownApp)
                        {
                            case KnownApp.OctaneRenderStandalone:
                                {
                                    imageBounds = Models.AppsInterop.OctaneRenderWindow.FindRenderedImageBorders(flatImage);
                                    break;
                                }
                            default: // TODO: paint.net and IrfanView specific support
                                {
                                    imageBounds = flatImage.FindBoundsOfInnerImage();
                                    break;
                                }
                        }

                        return new Models.GridTargetLocation
                        {
                            Offset = new Models.Geometry.Point(selectedNativeWindow.Location.X, selectedNativeWindow.Location.Y),
                            ImageBounds = imageBounds,
                        };
                    })
                    .ContinueWith((t) =>
                    {
                        if ((!t.Result.ImageBounds.IsEmpty) &&
                            (t.Result.ImageBounds.Width > 150) &&
                            (t.Result.ImageBounds.Height > 50))
                        {
                            this.SetWindowLocation(t.Result);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        enum KnownApp
        {
            Unknown,
            OctaneRenderStandalone,
            PaintDotNet,
            IrfanView,
        }

        // TODO: refactor - move to model
        public static Models.NativeWindowState SelectMainWindow(IList<Models.NativeWindowState> nativeWindows, string className)
        {
            // Fixing issue with detached panels
            var maxSizeWindow = nativeWindows
                .Where(w => String.Compare(w.ClassName, className, StringComparison.Ordinal) == 0)
                .OrderByDescending(w => w.Width * w.Height)
                .First();

            // Select window with max size from topmost list
            for (var i = 0; i < nativeWindows.Count; i++)
            {
                if (nativeWindows[i].ClassName != className)
                {
                    return null;
                }
                else if (nativeWindows[i].Handle == maxSizeWindow.Handle)
                {
                    return nativeWindows[i];
                }
            }

            return null;
        }

        private void SetWindowLocation(Models.GridTargetLocation location)
        {
            const int BufferSize = 1;

            var diffX = 0.0;
            var diffY = HeaderHeight;

            this.WindowLeft = location.ImageBounds.Left + location.Offset.X - diffX - BufferSize;
            this.WindowTop = location.ImageBounds.Top + location.Offset.Y - diffY - BufferSize;
            this.WindowWidth = (location.ImageBounds.Right - location.ImageBounds.Left) + diffX + 2 * BufferSize;
            this.WindowHeight = (location.ImageBounds.Bottom - location.ImageBounds.Top) + diffY + 2 * BufferSize;
        }
    }
}
