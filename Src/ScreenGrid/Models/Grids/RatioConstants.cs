using System;

namespace ScreenGrid.Models.Grids
{
    static class RatioConstants
    {
        public static readonly double Zero = 0.0;

        public static readonly double One = 1.0;

        public static readonly double Two = 2.0;

        public static readonly double Three = 3.0;

        /// <summary>
        /// 1.618 (Golden ratio).
        /// </summary>
        public static readonly double Phi = (1.0 + Math.Sqrt(5.0)) / 2.0;

        public static readonly double Phi5D8 = 1.0 / Phi;

        public static readonly double Phi3D8 = 1.0 - Phi5D8;

        public static readonly double Half = 1.0 / 2.0;

        public static readonly double OneThird = 1.0 / 3.0;

        public static readonly double TwoThirds = 2.0 / 3.0;

        public static readonly double OneSixth = 1.0 / 6.0;

        /// <summary>
        /// 1.5.
        /// </summary>
        public static readonly double OneDotFive = RatioConstants.Three / RatioConstants.Two;

        public static readonly double RootPhi = Math.Sqrt(Phi);

        public static readonly double Root2 = Math.Sqrt(2.0);

        public static readonly double Root3 = Math.Sqrt(3.0);

        public static readonly double Root4 = Math.Sqrt(4.0);

        public static readonly double Root5 = Math.Sqrt(5.0);

        /// <summary>
        /// 1.358456.
        /// </summary>
        public static readonly double GoldenSpiralCInRadians = Math.Pow(RatioConstants.Phi, 2.0 / Math.PI);
    }
}
