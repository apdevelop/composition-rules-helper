namespace ScreenGrid.Models.Geometry
{
    public struct Line
    {
        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Line(double x1, double y1, double x2, double y2)
        {
            this.p1 = new Point(x1, y1);
            this.p2 = new Point(x2, y2);
        }

        public Point p1; // { get; set; }

        public Point p2; // { get; set; }
    }
}
