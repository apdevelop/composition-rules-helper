namespace ScreenGrid.Models.Geometry
{
    using System;

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

        public double Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

        public double Top
        {
            get
            {
                return this.Y;
            }
        }

        public double Left
        {
            get
            {
                return this.X;
            }
        }

        public double Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }

        public double CenterX
        {
            get
            {
                return this.X + this.Width / 2.0;
            }
        }

        public double CenterY
        {
            get
            {
                return this.Y + this.Height / 2.0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return
                    this.X == Double.PositiveInfinity &&
                    this.Y == Double.PositiveInfinity &&
                    this.Width == System.Double.NegativeInfinity &&
                    this.Height == System.Double.NegativeInfinity;
            }
        }
    }
}
