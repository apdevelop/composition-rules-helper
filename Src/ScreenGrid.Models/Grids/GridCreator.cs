﻿namespace ScreenGrid.Models.Grids
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ScreenGrid.Models.Geometry;

    public static class GridCreator
    {
        public static Line[] Transform(
            IEnumerable<Line> src,
            Rotation rotation,
            bool isFlippedHorizontal,
            bool isFlippedVertical,
            double width,
            double height)
        {
            var lines = src.ToArray<Line>();
            var res = new Line[lines.Length];

            for (var i = 0; i < lines.Length; i++)
            {
                var newp1 = TransformPoint(lines[i].p1, rotation, isFlippedHorizontal, isFlippedVertical, width, height);
                var newp2 = TransformPoint(lines[i].p2, rotation, isFlippedHorizontal, isFlippedVertical, width, height);
                res[i] = new Line(newp1, newp2);
            }

            return res;
        }

        public static Point TransformPoint(
            Point point,
            Rotation rotation,
            bool isFlippedHorizontal,
            bool isFlippedVertical,
            double width,
            double height)
        {
            var res = new Point(point.X, point.Y);

            if (rotation != Rotation.R0)
            {
                var angle = Rotator.RotationAngle(rotation);

                var cx = 0.5;
                var cy = 0.5;
                res.X -= cx;
                res.Y -= cy;
                var xnew = (res.X * Math.Cos(angle)) - (res.Y * Math.Sin(angle));
                var ynew = (res.X * Math.Sin(angle)) + (res.Y * Math.Cos(angle));
                res.X = xnew + cx;
                res.Y = ynew + cy;
            }

            // Flipping
            if (isFlippedHorizontal)
            {
                res.X = 1.0 - res.X;
            }

            if (isFlippedVertical)
            {
                res.Y = 1.0 - res.Y;
            }

            // Denormalize (to screen coordinate system)
            res.X *= width;
            res.Y *= height;

            return res;
        }

        /// <summary>
        /// Creates list of lines, which forms the selected grid type
        /// X axis is directed from left to right
        /// Y axis is directed from top to bottom
        /// Coordinates are in normalized range [0..1]
        /// </summary>
        /// <param name="gridType">Selected type of grid</param>
        /// <param name="width">Width of output image</param>
        /// <param name="height">Height of output image</param>
        /// <param name="isRotated">Output grid is rotated (width swapped with height)</param>
        /// <returns>List of lines</returns>
        public static IEnumerable<Line> CreateGrid(GridType gridType, double width, double height, bool isRotated)
        {
            switch (gridType)
            {
                case GridType.None:
                    {
                        return new List<Line>();
                    }

                case GridType.Crosshair:
                    {
                        const double HorizontalTickLength = (3.0 / 100.0);
                        const double VerticalTickLength = (1.0 / 30.0);
                        const double CentralTickLength = (3.0 / 200.0);

                        var aspect = (width / height);

                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.Half), new Point(RatioConstants.Zero + HorizontalTickLength, RatioConstants.Half),
                                new Point(RatioConstants.One, RatioConstants.Half), new Point(RatioConstants.One - HorizontalTickLength, RatioConstants.Half),
                                new Point(RatioConstants.Half, RatioConstants.One), new Point(RatioConstants.Half, RatioConstants.One - VerticalTickLength),
                                new Point(RatioConstants.Half, RatioConstants.Zero), new Point(RatioConstants.Half, RatioConstants.Zero + VerticalTickLength),
                                new Point(RatioConstants.Half - CentralTickLength, RatioConstants.Half), new Point(RatioConstants.Half + CentralTickLength, RatioConstants.Half),
                                new Point(RatioConstants.Half, RatioConstants.Half - CentralTickLength * aspect), new Point(RatioConstants.Half, RatioConstants.Half + CentralTickLength * aspect),
                            };

                        return CreateLines(points);
                    }

                case GridType.Thirds:
                    {
                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.OneThird), new Point(RatioConstants.One, RatioConstants.OneThird),
                                new Point(RatioConstants.Zero, RatioConstants.TwoThirds), new Point(RatioConstants.One, RatioConstants.TwoThirds),
                                new Point(RatioConstants.OneThird, RatioConstants.Zero), new Point(RatioConstants.OneThird, RatioConstants.One),
                                new Point(RatioConstants.TwoThirds, RatioConstants.Zero), new Point(RatioConstants.TwoThirds, RatioConstants.One),
                            };

                        return CreateLines(points);
                    }

                case GridType.DiagonalOfThirds:
                    {
                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.One), new Point(RatioConstants.One, RatioConstants.Zero),
                                new Point(RatioConstants.Zero, RatioConstants.One - (RatioConstants.OneSixth)), new Point(RatioConstants.One - (RatioConstants.OneSixth), RatioConstants.Zero),
                                new Point(RatioConstants.OneSixth, RatioConstants.One), new Point(RatioConstants.One, RatioConstants.OneSixth),
                            };

                        return CreateLines(points);
                    }

                case GridType.GoldenRatio:
                    {
                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.Phi3D8), new Point(RatioConstants.One, RatioConstants.Phi3D8),
                                new Point(RatioConstants.Zero, RatioConstants.Phi5D8), new Point(RatioConstants.One, RatioConstants.Phi5D8),
                                new Point(RatioConstants.Phi3D8, RatioConstants.Zero), new Point(RatioConstants.Phi3D8, RatioConstants.One),
                                new Point(RatioConstants.Phi5D8, RatioConstants.Zero), new Point(RatioConstants.Phi5D8, RatioConstants.One),
                            };

                        return CreateLines(points);
                    }

                case GridType.GoldenTriangle:
                    {
                        var srcPoints = new List<Point>(6);
                        srcPoints.Add(new Point(RatioConstants.Zero, RatioConstants.Zero));
                        srcPoints.Add(new Point(RatioConstants.One, RatioConstants.One));

                        // TODO: if ((this.rotation == Rotation.R0) || (this.rotation == Rotation.R180))
                        {
                            var pt1 = new Point(0.0, height);
                            var pt2 = new Point(width, 0.0);

                            {
                                var pt4 = Models.Geometry.Utils.PerpendicularIntersection(pt1, pt2, Point.Zero);
                                pt4.Y = height - pt4.Y;

                                srcPoints.Add(new Point(pt1.X / width, pt1.Y / height));
                                srcPoints.Add(new Point(pt4.X / width, pt4.Y / height));
                            }

                            {
                                var pt4 = Models.Geometry.Utils.PerpendicularIntersection(pt1, pt2, new Point(width, height));
                                pt4.Y = height - pt4.Y;

                                srcPoints.Add(new Point(pt2.X / width, pt2.Y / height));
                                srcPoints.Add(new Point(pt4.X / width, pt4.Y / height));
                            }
                        }

                        return CreateLines(srcPoints.ToArray());
                    }

                case GridType.GoldenDiagonal1:
                    {
                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.One), new Point(RatioConstants.One, RatioConstants.Zero),
                                new Point(RatioConstants.Zero, RatioConstants.One), new Point(RatioConstants.Phi3D8, RatioConstants.Zero),
                                new Point(RatioConstants.Phi5D8, RatioConstants.One), new Point(RatioConstants.One, RatioConstants.Zero),
                            };

                        return CreateLines(points);
                    }

                case GridType.GoldenDiagonal2:
                    {
                        var points = new[]
                            {
                                new Point(RatioConstants.Zero, RatioConstants.One), new Point(RatioConstants.One, RatioConstants.Zero),
                                new Point(RatioConstants.Zero, RatioConstants.Zero), new Point(RatioConstants.Phi3D8, RatioConstants.One),
                                new Point(RatioConstants.Phi5D8, RatioConstants.Zero), new Point(RatioConstants.One, RatioConstants.One),
                            };

                        return CreateLines(points);
                    }

                case GridType.FibonacciRectanglesZoomed:
                case GridType.FibonacciRectanglesStretched:
                    {
                        // https://en.wikipedia.org/wiki/Fibonacci_number
                        // http://stackoverflow.com/a/18121331/1182448

                        // Current Fibonacci numbers
                        var current = 1;
                        var previous = 0;

                        // Current bounding box
                        var left = 0;
                        var right = 1;
                        var top = 0;
                        var bottom = 0;

                        const int NumberOfRectangles = 10;

                        var lines = new List<Line>();
                        for (var i = 0; i < NumberOfRectangles; i++)
                        {
                            switch (i % 4)
                            {
                                case 0: // attach to bottom of current rectangle
                                    lines.AddRange(CreateRectangleLines(left, right, bottom, bottom + current));
                                    bottom += current;
                                    break;
                                case 1: // attach to right of current rectangle
                                    lines.AddRange(CreateRectangleLines(right, right + current, top, bottom));
                                    right += current;
                                    break;
                                case 2: // attach to top of current rectangle
                                    lines.AddRange(CreateRectangleLines(left, right, top - current, top));
                                    top -= current;
                                    break;
                                case 3: // attach to left of current rectangle
                                    lines.AddRange(CreateRectangleLines(left - current, left, top, bottom));
                                    left -= current;
                                    break;
                            }

                            // TODO: create precalculated (Lazy) array of Fibonnacci numbers
                            // Update current fibonacci number
                            {
                                var temp = current;
                                current += previous;
                                previous = temp;
                            }
                        }

                        if (gridType == GridType.FibonacciRectanglesZoomed)
                        {
                            var aspect = (isRotated) ? (height / width) : (width / height);
                            lines = StretchToRectangleWithAspectRatio(lines, aspect, left, right, top, bottom);
                        }
                        else if (gridType == GridType.FibonacciRectanglesStretched)
                        {
                            lines = StretchToUniformRectangle(lines, left, top, right, bottom);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(gridType.ToString());
                        }

                        return lines;
                    }

                case GridType.GoldenSpiralZoomed:
                case GridType.GoldenSpiralStretched:
                    {
                        // https://en.wikipedia.org/wiki/Golden_spiral
                        // http://csharphelper.com/blog/2012/05/draw-a-phi-spiral-or-golden-spiral-in-c/

                        // TODO: Simplify code (use some pre-calculations and common functions)
                        var points = new List<Point>();

                        const double CenterX = 0.5;
                        const double CenterY = 0.5;
                        var minX = CenterX;
                        var minY = CenterY;
                        var maxX = CenterX;
                        var maxY = CenterY;

                        const int NumberOfTurns = 3;
                        const double FullTurn = 2.0 * Math.PI;
                        const double MaxAngle = ((double)NumberOfTurns) * FullTurn;

                        for (var theta = 0.0; theta < MaxAngle; )
                        {
                            var r = Math.Pow(RatioConstants.GoldenSpiralCInRadians, theta);

                            var x = CenterX + r * Math.Cos(theta);
                            var y = 1.0 - (CenterY + r * Math.Sin(theta)); // Flip Y axis

                            points.Add(new Point(x, y));

                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);

                            // variable step for optimization
                            var turnNumber = theta / FullTurn;
                            var step = 0.005 * 5.0 * ((double)NumberOfTurns) / (turnNumber + 1.0) * FullTurn;
                            theta += step;
                        }

                        // Add some points after main turns to finish spiral (inscribe to rectangle)
                        var lastY = minY;
                        for (var theta = MaxAngle; ; theta += 0.001 * FullTurn)
                        {
                            var r = Math.Pow(RatioConstants.GoldenSpiralCInRadians, theta);

                            var x = CenterX + r * Math.Cos(theta);
                            var y = 1.0 - (CenterY + r * Math.Sin(theta)); // Flip Y axis

                            points.Add(new Point(x, y));

                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);

                            if (y <= lastY)
                            {
                                // TODO: Correct last point (clip to bounds)
                                break;
                            }
                        }

                        var lines = new List<Line>();
                        for (var i = 1; i < points.Count; i++)
                        {
                            lines.Add(new Line(points[i - 1], points[i]));
                        }

                        if (gridType == GridType.GoldenSpiralZoomed)
                        {
                            var aspect = (isRotated) ? (height / width) : (width / height);
                            lines = StretchToRectangleWithAspectRatio(lines, aspect, minX, maxX, minY, maxY);

                            // Adding extents lines
                            var extents = GetExtents(lines);
                            if (aspect < RatioConstants.Phi)
                            {
                                lines.Add(new Line(RatioConstants.Zero, extents.Top, RatioConstants.One, extents.Top));
                                lines.Add(new Line(RatioConstants.Zero, extents.Bottom, RatioConstants.One, extents.Bottom));
                            }
                            else
                            {
                                lines.Add(new Line(extents.Left, RatioConstants.Zero, extents.Left, RatioConstants.One));
                                lines.Add(new Line(extents.Right, RatioConstants.Zero, extents.Right, RatioConstants.One));
                            }
                        }
                        else if (gridType == GridType.GoldenSpiralStretched)
                        {
                            lines = StretchToUniformRectangle(lines, minX, minY, maxX, maxY);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(gridType.ToString());
                        }

                        return lines;
                    }
                default:
                    {
                        throw new ArgumentException(gridType.ToString());
                    }
            }
        }

        private static List<Line> StretchToRectangleWithAspectRatio(List<Line> lines, double aspect, double left, double right, double top, double bottom)
        {
            if (aspect < RatioConstants.Phi)
            {
                lines = StretchToRectangle(1.0, (RatioConstants.Phi - 1.0) * aspect, lines, left, top, right, bottom);
                lines = AlignToCenter(lines);
            }
            else
            {
                lines = StretchToRectangle(RatioConstants.Phi / aspect, 1.0, lines, left, top, right, bottom);
                lines = AlignToCenter(lines);
            }

            return lines;
        }

        /// <summary>
        /// Scale and auto-fit set of lines (to 0..1 output range)
        /// Min/max values passed as precalculated parameters for optimization
        /// </summary>
        /// <param name="lines">List of lines to stretch</param>
        /// <param name="minX">Minimum X of points of lines</param>
        /// <param name="minY">Minimum Y of points of lines</param>
        /// <param name="maxX">Maximum X of points of lines</param>
        /// <param name="maxY">Maximum Y of points of lines</param>
        /// <returns></returns>
        private static List<Line> StretchToUniformRectangle(IEnumerable<Line> lines, double minX, double minY, double maxX, double maxY)
        {
            return StretchToRectangle(1.0, 1.0, lines, minX, minY, maxX, maxY);
        }

        private static List<Line> StretchToRectangle(double width, double height, IEnumerable<Line> lines, double minX, double minY, double maxX, double maxY)
        {
            var scaleX = width / (maxX - minX);
            var scaleY = height / (maxY - minY);
            var offsetX = minX;
            var offsetY = minY;

            return lines.Select(r => new Line(
                     (r.p1.X - offsetX) * scaleX, (r.p1.Y - offsetY) * scaleY,
                     (r.p2.X - offsetX) * scaleX, (r.p2.Y - offsetY) * scaleY))
                    .ToList();
        }

        private static Rectangle GetExtents(IEnumerable<Line> lines)
        {
            var p = lines.First().p1;
            var minX = p.X;
            var minY = p.Y;
            var maxX = p.X;
            var maxY = p.Y;

            foreach (var line in lines)
            {
                minX = Math.Min(Math.Min(minX, line.p1.X), Math.Min(minX, line.p2.X));
                minY = Math.Min(Math.Min(minY, line.p1.Y), Math.Min(minY, line.p2.Y));
                maxX = Math.Max(Math.Max(maxX, line.p1.X), Math.Max(maxX, line.p2.X));
                maxY = Math.Max(Math.Max(maxY, line.p1.Y), Math.Max(maxY, line.p2.Y));
            }

            return new Rectangle { X = minX, Y = minY, Width = maxX - minX, Height = maxY - minY };
        }

        private static List<Line> AlignToCenter(IEnumerable<Line> lines)
        {
            var extents = GetExtents(lines);

            var offsetX = (1.0 - extents.Width) / 2.0;
            var offsetY = (1.0 - extents.Height) / 2.0;

            return lines.Select(r => new Line(
                     new Point((r.p1.X + offsetX), (r.p1.Y + offsetY)),
                     new Point((r.p2.X + offsetX), (r.p2.Y + offsetY))))
                    .ToList();
        }

        private static IList<Line> CreateRectangleLines(int left, int right, int top, int bottom)
        {
            return new List<Line>(new[]
            {
                new Line(new Point(left, bottom), new Point(right, bottom)),
                new Line(new Point(right, bottom), new Point(right, top)),
                new Line(new Point(right, top), new Point(left, top)),
                new Line(new Point(left, top), new Point(left, bottom)),
            });
        }

        private static IEnumerable<Line> CreateLines(Point[] points)
        {
            var res = new List<Line>();

            for (var i = 0; i < points.Length; i += 2)
            {
                res.Add(new Line(new Point(points[i].X, points[i].Y), new Point(points[i + 1].X, points[i + 1].Y)));
            }

            return res;
        }
    }
}
