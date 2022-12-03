using System;

namespace ScreenGrid.Models.Geometry
{
    public struct Rectangle
    {
        // TODO: process infinite values

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public static Rectangle Empty
        {
            get
            {
                return new Rectangle
                {
                    X = Double.PositiveInfinity,
                    Y = Double.PositiveInfinity,
                    Width = System.Double.NegativeInfinity,
                    Height = System.Double.NegativeInfinity,
                };
            }
        }

        public double Right => this.X + this.Width;

        public double Top => this.Y;

        public double Left => this.X;

        public double Bottom => this.Y + this.Height;

        public double CenterX => this.X + this.Width / 2.0;

        public double CenterY => this.Y + this.Height / 2.0;

        public bool IsEmpty
        {
            get
            {
                return
                    this.X == Double.PositiveInfinity &&
                    this.Y == Double.PositiveInfinity &&
                    this.Width == Double.NegativeInfinity &&
                    this.Height == Double.NegativeInfinity;
            }
        }
    }
}
