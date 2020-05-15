namespace ScreenGrid.Models.Grids
{
    using System.Collections.Generic;

    public class GridModeItem
    {
        private readonly GridType gridType;

        private readonly string title;

        private readonly bool flipEnabled;

        private readonly bool rotateEnabled;

        public GridModeItem(GridType gridType, string title, bool flipEnabled, bool rotateEnabled)
        {
            this.gridType = gridType;
            this.title = title;
            this.flipEnabled = flipEnabled;
            this.rotateEnabled = rotateEnabled;
        }

        public GridType GridType
        {
            get
            {
                return this.gridType;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
        }

        public bool IsFlipEnabled
        {
            get
            {
                return this.flipEnabled;
            }
        }

        public bool IsRotateEnabled
        {
            get
            {
                return this.rotateEnabled;
            }
        }

        public static IEnumerable<GridModeItem> List
        {
            get
            {
                return new[]
                {
                    new GridModeItem(GridType.Crosshair, "Crosshair", false, false),
                    new GridModeItem(GridType.Thirds, "Thirds", false, false),
                    new GridModeItem(GridType.GoldenRatio, "Golden Ratio", false, false),
                    new GridModeItem(GridType.DiagonalOfThirds,"Diagonal of Thirds", true, true),
                    new GridModeItem(GridType.GoldenTriangle, "Golden Triangle", true, true),
                    new GridModeItem(GridType.GoldenDiagonal1, "Golden Diagonal 1", true, true),
                    new GridModeItem(GridType.GoldenDiagonal2, "Golden Diagonal 2", true, true),
                    new GridModeItem(GridType.FibonacciRectanglesZoomed, "Fibonacci Rectangles", true, true),
                    new GridModeItem(GridType.GoldenSpiralZoomed, "Golden Spiral", true, true),
                    new GridModeItem(GridType.FibonacciRectanglesStretched, "Fibonacci Rectangles (stretched)", true, true),
                    new GridModeItem(GridType.GoldenSpiralStretched, "Golden Spiral (stretched)", true, true),
                    new GridModeItem(GridType.GoldenCircles, "Golden Circles", false, true),
                    new GridModeItem(GridType.OneDotFiveRectangle, "1.5 Rectangle", false, true),
                    new GridModeItem(GridType.GoldenRectangle, "Golden (Phi) Rectangle", false, true),
                    new GridModeItem(GridType.RootPhiRectangle, "Root-Phi Rectangle", false, true),
                    new GridModeItem(GridType.Root2Rectangle, "Root-2 Rectangle", false, true),
                    new GridModeItem(GridType.Root3Rectangle, "Root-3 Rectangle", false, true),
                    new GridModeItem(GridType.Root4Rectangle, "Root-4 Rectangle", false, true),
                    new GridModeItem(GridType.Root5Rectangle, "Root-5 Rectangle", false, true),
                    new GridModeItem(GridType.Armature14Line, "Armature (14 Line)", false, false),
                };
            }
        }
    }
}