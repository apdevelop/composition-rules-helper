﻿namespace ScreenGrid.Models.Grids
{
    using System;
    using System.Collections.Generic;

    public class GridModeItem
    {
        private GridType gridType;

        private string title;

        public GridModeItem(GridType gridType, string title)
        {
            this.gridType = gridType;
            this.title = title;
        }

        public GridType GridMode
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

        public static IEnumerable<GridModeItem> List
        {
            get
            {
                return new[]
                {
                    new GridModeItem(GridType.Crosshair, "Crosshair"),
                    new GridModeItem(GridType.Thirds, "Thirds"),
                    new GridModeItem(GridType.GoldenRatio, "Golden Ratio"),
                    new GridModeItem(GridType.DiagonalOfThirds,"Diagonal of Thirds"),
                    new GridModeItem(GridType.GoldenTriangle, "Golden Triangle"),
                    new GridModeItem(GridType.GoldenDiagonal1, "Golden Diagonal 1"),
                    new GridModeItem(GridType.GoldenDiagonal2, "Golden Diagonal 2"),
                    new GridModeItem(GridType.FibonacciRectanglesZoomed, "Fibonacci Rectangles"),
                    new GridModeItem(GridType.GoldenSpiralZoomed, "Golden Spiral"),
                    new GridModeItem(GridType.FibonacciRectanglesStretched, "Fibonacci Rectangles (stretched)"),
                    new GridModeItem(GridType.GoldenSpiralStretched, "Golden Spiral (stretched)"),
                };
            }
        }
    }
}
