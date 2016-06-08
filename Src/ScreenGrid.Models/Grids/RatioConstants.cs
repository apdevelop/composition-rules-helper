namespace ScreenGrid.Models.Grids
{
    using System;

    public static class RatioConstants
    {
        public static readonly double Phi = (1.0 + Math.Sqrt(5.0)) / 2.0; 

        public static readonly double Phi5D8 = 1.0 / Phi;
       
        public static readonly double Phi3D8 = 1.0 - Phi5D8;

        public static readonly double OneThird = 1.0 / 3.0;

        public static readonly double TwoThirds = 2.0 / 3.0;

        /// <summary>
        /// 1.358456
        /// </summary>
        public static readonly double GoldenSpiralCInRadians = Math.Pow(RatioConstants.Phi, 2.0 / Math.PI);
    }
}