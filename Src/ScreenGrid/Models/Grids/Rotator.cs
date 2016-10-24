namespace ScreenGrid.Models.Grids
{
    using System;

    public static class Rotator
    {
        public static Rotation RotateCounterClockwise(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.R0: return Rotation.R90;
                case Rotation.R90: return Rotation.R180;
                case Rotation.R180: return Rotation.R270;
                case Rotation.R270: return Rotation.R0;
                default: throw new ArgumentException(rotation.ToString());
            }
        }

        public static Rotation RotateClockwise(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.R0: return Rotation.R270;
                case Rotation.R90: return Rotation.R0;
                case Rotation.R180: return Rotation.R90;
                case Rotation.R270: return Rotation.R180;
                default: throw new ArgumentException(rotation.ToString());
            }
        }

        public static double RotationAngle(Rotation r)
        {
            switch (r)
            {
                case Rotation.R0:
                    {
                        return 0.0;
                    }
                case Rotation.R90:
                    {
                        return Math.PI / 2.0;
                    };
                case Rotation.R180:
                    {
                        return Math.PI;
                    };
                case Rotation.R270:
                    {
                        return 3.0 / 2.0 * Math.PI;
                    };
                default:
                    {
                        throw new ArgumentException(r.ToString());
                    }
            }
        }
    }
}
