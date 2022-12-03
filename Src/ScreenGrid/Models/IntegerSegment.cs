using System;
using System.Globalization;

namespace ScreenGrid.Models
{
    /// <summary>
    /// Representation of segment.
    /// </summary>
    public struct IntegerSegment
    {
        private int start;

        private int end;

        public IntegerSegment(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int Start
        {
            get { return this.start; }
            set { this.start = value; }
        }

        public int End
        {
            get { return this.end; }
            set { this.end = value; }
        }

        public static IntegerSegment Zero => new IntegerSegment(0, 0);

        public override bool Equals(Object obj) => (obj is IntegerSegment) && (this == (IntegerSegment)obj);

        public override int GetHashCode() => start.GetHashCode() ^ end.GetHashCode();

        public static bool operator ==(IntegerSegment x, IntegerSegment y) => (x.start == y.start) && (x.end == y.end);

        public static bool operator !=(IntegerSegment x, IntegerSegment y) => !(x == y);

        public override string ToString() => String.Format(CultureInfo.InvariantCulture, "[{0} {1}]", this.Start, this.End);
    }
}
