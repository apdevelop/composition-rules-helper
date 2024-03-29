﻿namespace ScreenGrid.Models.Grids
{
    /// <summary>
    /// Grid type.
    /// </summary>
    public enum GridType
    {
        /// <summary>
        /// No grid is displayed.
        /// </summary>
        None,

        /// <summary>
        /// Simple crosshair.
        /// </summary>
        Crosshair,

        /// <summary>
        /// Rule of Thirds simple grid.
        /// </summary>
        Thirds,

        /// <summary>
        /// One side of the picture is divided into two, and then each half is divided into three parts. 
        /// The adjacent side is divided so that the lines connecting the resulting points form a diagonal frame. 
        /// According to the Diagonal Rule, important elements of the picture should be placed along these
        /// diagonals.
        /// </summary>
        DiagonalOfThirds,

        /// <summary>
        /// Golden Ratio simple grid.
        /// </summary>
        GoldenRatio,

        /// <summary>
        /// Main diagonal with two perpendiculars from corners.
        /// </summary>
        GoldenTriangle,

        /// <summary>
        /// Main diagonal with two additional diagonals to Golden Ratio points (Type 1).
        /// </summary>
        GoldenDiagonal1,

        /// <summary>
        /// Main diagonal with two additional diagonals to Golden Ratio points (Type 2).
        /// </summary
        GoldenDiagonal2,

        /// <summary>
        /// Sequence of Fibonacci Rectangles.
        /// </summary>
        FibonacciRectanglesZoomed,

        /// <summary>
        /// Sequence of Fibonacci Rectangles (stretched to area extents).
        /// </summary>
        FibonacciRectanglesStretched,

        /// <summary>
        /// Golden Spiral.
        /// </summary>
        GoldenSpiralZoomed,

        /// <summary>
        /// Golden Spiral (stretched to area extents).
        /// </summary>
        GoldenSpiralStretched,

        /// <summary>
        /// Two pairs of circles.
        /// </summary>
        GoldenCircles,

        /// <summary>
        /// 1.5 Rectangle (1:1.5 or 3:2).
        /// </summary>
        OneDotFiveRectangle,

        /// <summary>
        /// Golden rectangle or Auron (1:φ).
        /// </summary>
        GoldenRectangle,

        /// <summary>
        /// Root-Phi or Penton (1:√φ).
        /// </summary>
        RootPhiRectangle,

        /// <summary>
        /// Root-2 or Diagon (1:√2).
        /// </summary>
        Root2Rectangle,

        /// <summary>
        /// Root-3.
        /// </summary>
        Root3Rectangle,

        /// <summary>
        /// Root-4.
        /// </summary>
        Root4Rectangle,

        /// <summary>
        /// Root-5.
        /// </summary>
        Root5Rectangle,

        /// <summary>
        /// Armature (14 Line).
        /// </summary>
        Armature14Line,
    }
}
