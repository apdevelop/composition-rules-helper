namespace ScreenGrid.Models.Grids
{
    using System;

    static class RatioConstants
    {
        public static readonly double Zero = 0.0;

        public static readonly double One = 1.0;

        /// <summary>
        /// 1.618
        /// </summary>
        public static readonly double Phi = (1.0 + Math.Sqrt(5.0)) / 2.0; 

        public static readonly double Phi5D8 = 1.0 / Phi;
       
        public static readonly double Phi3D8 = 1.0 - Phi5D8;

        public static readonly double Half = 1.0 / 2.0;

        public static readonly double OneThird = 1.0 / 3.0;

        public static readonly double TwoThirds = 2.0 / 3.0;

        public static readonly double OneSixth = 1.0 / 6.0;

        /// <summary>
        /// 1.358456
        /// </summary>
        public static readonly double GoldenSpiralCInRadians = Math.Pow(RatioConstants.Phi, 2.0 / Math.PI);
    }
}