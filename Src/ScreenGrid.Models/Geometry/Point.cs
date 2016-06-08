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

        public static Point Zero
        {
            get
            {
                return new Point(0.0, 0.0);
            }
        }
    }
}
