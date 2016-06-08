namespace ScreenGrid.Models.Geometry
{
    public struct Line
    {
        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Point p1; // { get; set; }
        public Point p2; // { get; set; }
    }
}
