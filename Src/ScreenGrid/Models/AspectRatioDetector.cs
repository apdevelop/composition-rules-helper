using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenGrid.Models
{
    public static class AspectRatioDetector
    {
        // TODO: add tests

        /// <summary>
        /// Common aspect ratios
        /// https://en.wikipedia.org/wiki/Aspect_ratio_%28image%29
        /// </summary>
        private static readonly Dictionary<double, string> commonAspectRatios = new Dictionary<double, string>()
        {
            // TODO: create class with separated properties
            // TODO: add descriptions for displaying in tooltip
            { 1.0, "1:1" },
            { 6.0 / 5.0, "6:5" },
            { 5.0 / 4.0, "5:4" },
            { 4.0 / 3.0, "4:3" },
            { 11.0 / 8.0, "11:8" },
            { Math.Sqrt(2.0), "1.41:1" }, // ISO 216 paper sizes
            { 1.43, "1.43:1" },
            { 3.0 / 2.0, "3:2" },
            { 16.0 / 10.0, "16:10" }, // The golden ratio
            { 1.618, "16.18:10" },
            { 5.0 / 3.0, "5:3" },
            { 16.0 / 9.0, "16:9" },
            { 1.85, "1.85:1" },
            { 2.00, "2:1" },
            { 2.35, "2.35:1" },
            { 2.39, "2.39:1" },
            { 2.414, "2.414:1" }, // The silver ratio
            { 2.76, "2.76:1" },
        };

        public static string DetectCommonAspectRatio(double ratio)
        {
            const double Tolerance = 0.01;

            if (ratio >= 1.0)
            {
                // Horizontal
                var nearest = commonAspectRatios.OrderBy(kvp => Math.Abs(kvp.Key - ratio)).First();
                if (Math.Abs(nearest.Key - ratio) < Tolerance)
                {
                    return nearest.Value;
                }
            }
            else
            {
                // Vertical
                ratio = 1.0 / ratio;
                var nearest = commonAspectRatios.OrderBy(kvp => Math.Abs(kvp.Key - ratio)).First();
                if (Math.Abs(nearest.Key - ratio) < Tolerance)
                {
                    var arr = nearest.Value.Split(':');
                    return arr[1] + ":" + arr[0];
                }
            }

            return String.Empty;
        }
    }
}
