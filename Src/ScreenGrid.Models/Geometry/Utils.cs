namespace ScreenGrid.Models.Geometry
{
    public static class Utils
    {
        public static Point PerpendicularIntersection(Point pt1, Point pt2, Point pt3)
        {
            // http://www.cyberforum.ru/cpp-beginners/thread125838.html
            // http://stackoverflow.com/questions/1811549/perpendicular-on-a-line-from-a-given-point

            var x1 = pt1.X;
            var y1 = pt1.Y;
            var x2 = pt2.X;
            var y2 = pt2.Y;
            var x3 = pt3.X;
            var y3 = pt3.Y;

            var k = ((y2 - y1) * (x3 - x1) - (x2 - x1) * (y3 - y1)) / ((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));

            var x4 = x3 - k * (y2 - y1);
            var y4 = y3 + k * (x2 - x1);

            return new Point(x4, y4);
        }
    }
}
