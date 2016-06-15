namespace ScreenGrid.Models.Grids
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
        /// </summary>
        /// <param name="gridType">Selected type of grid</param>
        /// <param name="width">Width of output image</param>
        /// <param name="height">Height of output image</param>
        /// <returns>List of lines; points coordinates are in normalized range [0..1]</returns>
        public static IEnumerable<Line> CreateGrid(GridType gridType, double width, double height)
        {
            switch (gridType)
            {
                case GridType.None:
                    {
                        return new List<Line>();
                    }

                case GridType.Thirds:
                    {
                        var points = new[]
                            {
                                new Point(0.0, RatioConstants.OneThird),  new Point(1.0, RatioConstants.OneThird),
                                new Point(0.0, RatioConstants.TwoThirds),  new Point(1.0, RatioConstants.TwoThirds),
                                new Point(RatioConstants.OneThird, 0.0),  new Point(RatioConstants.OneThird, 1.0),
                                new Point(RatioConstants.TwoThirds, 0.0),  new Point(RatioConstants.TwoThirds, 1.0),
                            };

                        return CreateLines(points);
                    }

                case GridType.DiagonalOfThirds:
                    {
                        var points = new[]
                            {
                                new Point(0.0, 1.0),  new Point(1.0, 0.0),
                                new Point(0.0, 1.0 - (RatioConstants.OneThird / 2.0)),  new Point(1.0 - (RatioConstants.OneThird / 2.0), 0.0),
                                new Point(RatioConstants.OneThird / 2.0, 1.0),  new Point(1.0, RatioConstants.OneThird / 2.0),
                            };

                        return CreateLines(points);
                    }

                case GridType.GoldenRatio:
                    {
                        var points = new[]
                            {
                                new Point(0.0, RatioConstants.Phi3D8),  new Point(1.0, RatioConstants.Phi3D8),
                                new Point(0.0, RatioConstants.Phi5D8),  new Point(1.0, RatioConstants.Phi5D8),
                                new Point(RatioConstants.Phi3D8, 0.0),  new Point(RatioConstants.Phi3D8, 1.0),
                                new Point(RatioConstants.Phi5D8, 0.0),  new Point(RatioConstants.Phi5D8, 1.0),
                            };

                        return CreateLines(points);
                    }

                case GridType.GoldenTriangle:
                    {
                        var srcPoints = new List<Point>(6);
                        srcPoints.Add(new Point(0.0, 0.0));
                        srcPoints.Add(new Point(1.0, 1.0));

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

                case GridType.GoldenDiagonal:
                    {
                        var points = new[]
                            {
                                new Point(0.0, 1.0),  new Point(1.0, 0.0),
                                new Point(0.0, 0.0),  new Point(RatioConstants.Phi3D8, 1.0),
                                new Point(RatioConstants.Phi5D8, 0.0),  new Point(1.0, 1.0),
                            };

                        return CreateLines(points);
                    }

                case GridType.FibonacciRectangles:
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
                                    lines.AddRange(CreateRectangle(left, right, bottom, bottom + current));
                                    bottom += current;
                                    break;
                                case 1: // attach to right of current rectangle
                                    lines.AddRange(CreateRectangle(right, right + current, top, bottom));
                                    right += current;
                                    break;
                                case 2: // attach to top of current rectangle
                                    lines.AddRange(CreateRectangle(left, right, top - current, top));
                                    top -= current;
                                    break;
                                case 3: // attach to left of current rectangle
                                    lines.AddRange(CreateRectangle(left - current, left, top, bottom));
                                    left -= current;
                                    break;
                            }

                            // Update current fibonacci number
                            {
                                var temp = current;
                                current += previous;
                                previous = temp;
                            }
                        }

                        // Scale and auto-fit (stretch all lines to 0..1 output range)
                        var scaleX = 1.0 / ((double)(right - left));
                        var scaleY = 1.0 / ((double)(bottom - top));
                        var offsetX = left;
                        var offsetY = top;

                        lines = lines.Select(r => new Line(
                            new Point((r.p1.X - offsetX) * scaleX, (r.p1.Y - offsetY) * scaleY),
                            new Point((r.p2.X - offsetX) * scaleX, (r.p2.Y - offsetY) * scaleY)))
                            .ToList();

                        return lines;
                    }

                ////case GridType.GoldenSpiral:
                ////    {
                ////        // https://en.wikipedia.org/wiki/Golden_spiral
                ////        // http://csharphelper.com/blog/2012/05/draw-a-phi-spiral-or-golden-spiral-in-c/

                ////        var res = new List<Line>();

                ////        // TODO: better algorithm
                ////        var points = new List<Point>();

                ////        for (var theta = 0.0; theta < 3.0 * 2.0 * Math.PI; theta += 0.05 * Math.PI)
                ////        {
                ////            var r = 0.001 * Math.Pow(RatioConstants.GoldenSpiralCInRadians, theta);

                ////            var x = 0.5 + r * Math.Cos(theta);
                ////            var y = 0.5 + r * Math.Sin(theta);
                ////            points.Add(new Point(x, y));
                ////        }

                        ////const int num_slices = 1000;

                        ////var start = new Point(0, 1);
                        ////var origin = new Point(0.5, 0.5);
                        ////var dx = start.X - origin.X;
                        ////var dy = start.Y - origin.Y;
                        ////var radius = Math.Sqrt(dx * dx + dy * dy);
                        ////var theta = Math.Atan2(dy, dx);

                        ////var dtheta = Math.PI / 2.0 / num_slices;
                        ////var factor = 1 - (1 / RatioConstants.Phi) / num_slices * 0.78;

                        ////// Repeat until dist is too small to see.
                        ////while (radius > 0.01)
                        ////{
                        ////    points.Add(new Point(
                        ////        (origin.X + radius * Math.Cos(theta)),
                        ////        (origin.Y + radius * Math.Sin(theta))));

                        ////    theta += dtheta;
                        ////    radius *= factor;
                        ////}

                    ////    for (var i = 1; i < points.Count; i++)
                    ////    {
                    ////        res.Add(new Line(points[i - 1], points[i]));
                    ////    }

                    ////    return res;
                    ////}
                default:
                    {
                        throw new ArgumentException(gridType.ToString());
                    }
            }
        }

        private static IList<Line> CreateRectangle(int left, int right, int top, int bottom)
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
