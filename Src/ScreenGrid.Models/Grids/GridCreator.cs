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
        /// <returns>List of lines</returns>
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
                        var points = new[]
                            {
                                new Point(RatioConstants.Phi5D8, 0.0),  new Point(RatioConstants.Phi5D8, 1.0),
                                new Point(RatioConstants.Phi5D8, RatioConstants.Phi3D8),  new Point(1.0, RatioConstants.Phi3D8),

                                // v
                                new Point(RatioConstants.Phi5D8 + (RatioConstants.Phi3D8 * RatioConstants.Phi3D8), RatioConstants.Phi3D8),   
                                new Point(RatioConstants.Phi5D8 + (RatioConstants.Phi3D8 * RatioConstants.Phi3D8), 0.0),

                                // h
                                new Point(RatioConstants.Phi5D8 + (RatioConstants.Phi3D8 * RatioConstants.Phi3D8), RatioConstants.Phi3D8 * RatioConstants.Phi5D8),   
                                new Point(RatioConstants.Phi5D8, RatioConstants.Phi3D8 * RatioConstants.Phi5D8),

                                // v
                                new Point(RatioConstants.Phi5D8 + (RatioConstants.Phi3D8 * RatioConstants.Phi3D8 * RatioConstants.Phi5D8), RatioConstants.Phi3D8),   
                                new Point(RatioConstants.Phi5D8 + (RatioConstants.Phi3D8 * RatioConstants.Phi3D8 * RatioConstants.Phi5D8), RatioConstants.Phi3D8 * RatioConstants.Phi5D8),
                            };

                        return CreateLines(points);
                    }

                // TODO: case GridType.GoldenSpiral:
                ////{
                ////    var res = new List<Line>();

                ////    // TODO: better algorithm
                ////    var points = new List<Point>();
                ////    for (var theta = 0.0; theta < 12.0 * Math.PI; theta += 0.01 * Math.PI)
                ////    {
                ////        var r = 0.2;
                ////        r = 0.005 * Math.Pow(RatioConstants.GOLDEN_SPIRAL_C_RADIANS, theta);
                ////        var x = r * Math.Cos(theta);
                ////        var y = r * Math.Sin(theta);
                ////        points.Add(new Point(x + 0.5, y + 0.5));
                ////    }

                ////    for (var i = 1; i < points.Count; i++)
                ////    {
                ////        res.Add(new Line(new Point(points[i - 1].X, points[i - 1].Y), new Point(points[i].X, points[i].Y)));
                ////    }

                ////    return res;
                ////}
                default:
                    {
                        throw new ArgumentException(gridType.ToString());
                    }
            }
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
