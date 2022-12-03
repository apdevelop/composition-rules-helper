using System;
using System.Globalization;

namespace ScreenGrid.Models.Geometry
{
    public struct Point
    {
        private double x;

        private double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public static Point Zero => new Point(0.0, 0.0);

        public override string ToString() => String.Format(CultureInfo.InvariantCulture, "{0} {1}", this.X, this.Y);
    }
}
