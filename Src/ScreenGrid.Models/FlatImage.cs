﻿namespace ScreenGrid.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 'Flattened' image representation for fast processing
    /// </summary>
    public class FlatImage
    {
        // TODO: ? use single-dimension array ?
        public UInt32[,] pixels;

        public int Width
        {
            get
            {
                return pixels.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return pixels.GetLength(1);
            }
        }

        public FlatImage(System.Drawing.Bitmap bitmap)
        {
            this.pixels = FromBitmap(bitmap);
        }

        private static int MidDiff(UInt32 c1, UInt32 c2)
        {
            var rgb1 = BitConverter.GetBytes(c1);
            var rgb2 = BitConverter.GetBytes(c2);
            return (int)((
                ((double)Math.Abs(rgb1[0] - rgb2[0])) +
                ((double)Math.Abs(rgb1[1] - rgb2[1])) +
                ((double)Math.Abs(rgb1[2] - rgb2[2])) +
                ((double)Math.Abs(rgb1[3] - rgb2[3]))
                ) / 4.0);
        }

        public UInt32[] GetHorizontalStripe(int iy)
        {
            var result = new UInt32[this.Width];
            for (var ix = 0; ix < this.Width; ix++)
            {
                result[ix] = this.pixels[ix, iy];
            }

            return result;
        }

        // TODO: move to separate class

        public static UInt32[] GetDerivative(UInt32[] stripe)
        {
            var result = new UInt32[stripe.Length];

            result[0] = stripe[0];
            for (var i = 1; i < stripe.Length; i++)
            {
                result[i] = stripe[i] - stripe[i - 1];
            }

            return result;
        }

        /// <summary>
        /// Search for all-zero segments of array with given minimal length
        /// </summary>
        /// <param name="array">Input array</param>
        /// <param name="minimalSegmentLength">Minimal length of segment</param>
        /// <returns>List of segments (startIndex, endIndex) which contains only zeros</returns>
        public static IList<Tuple<int, int>> FindZeroSegments(UInt32[] array, int minimalSegmentLength)
        {
            var result = new List<Tuple<int, int>>();

            if (array.Length < minimalSegmentLength)
            {
                return result;
            }

            int lastZero = -1;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == 0)
                {
                    if (lastZero == -1)
                    {
                        lastZero = i;
                    }

                    if (i == (array.Length - 1))
                    {
                        var segment = new Tuple<int, int>(lastZero, i);
                        if (CheckSegmentLength(segment, minimalSegmentLength))
                        {
                            result.Add(segment);
                        }
                    }
                }
                else
                {
                    if (lastZero != -1)
                    {
                        var segment = new Tuple<int, int>(lastZero, i - 1);
                        lastZero = -1;
                        if (CheckSegmentLength(segment, minimalSegmentLength))
                        {
                            result.Add(segment);
                        }
                    }
                }
            }

            return result;
        }

        private static bool CheckSegmentLength(Tuple<int, int> segment, int minimalSegmentLength)
        {
            return ((segment.Item2 - segment.Item1 + 1) >= minimalSegmentLength);
        }

        public static Tuple<int, int> IntersectionOfSegments(Tuple<int, int> segment1, Tuple<int, int> segment2)
        {
            if ((segment1.Item1 > segment2.Item2) || (segment1.Item2 < segment2.Item1))
            {
                return null;
            }
            else
            {
                var start = Math.Max(segment1.Item1, segment2.Item1);
                var end = Math.Min(segment1.Item2, segment2.Item2);
                return new Tuple<int, int>(start, end);
            }
        }

        public static IList<Tuple<int, int>> IntersectionOfSegments(IEnumerable<IList<Tuple<int, int>>> lines)
        {
            var sum = new List<Tuple<int, int>>();
            foreach (var line in lines)
            {
                if (sum.Count == 0)
                {
                    sum.AddRange(line);
                }
                else
                { 
                    // Perform intersection checks
                    var newSum = new List<Tuple<int, int>>();
                    foreach (var segmentA in sum)
                    {
                        foreach (var segmentB in line)
                        {
                            var r = IntersectionOfSegments(segmentA, segmentB);
                            if (r != null)
                            {
                                newSum.Add(r);
                            }
                        }
                    }

                    sum = newSum;
                    if (sum.Count == 0)
                    {
                        break;
                    }
                }
            }

            return sum;
        }

        public bool CompareWithFragment(FlatImage fragment, int startX, int startY)
        {
            for (var x = 0; x < fragment.Width; x++)
            {
                for (var y = 0; y < fragment.Height; y++)
                {
                    if (this.pixels[x + startX, y + startY] != fragment.pixels[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CompareWithFragmentWithTolerance(FlatImage fragment, int startX, int startY)
        {
            for (var x = 0; x < fragment.Width; x++)
            {
                for (var y = 0; y < fragment.Height; y++)
                {
                    if (MidDiff(this.pixels[x + startX, y + startY], fragment.pixels[x, y]) > 2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static unsafe UInt32[,] FromBitmap(System.Drawing.Bitmap bitmap)
        {
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                throw new FormatException(String.Format("Image format '{0}' is not supported", bitmap.PixelFormat));
            }

            int pixelSize = 3;
            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var res = new UInt32[bitmapData.Width, bitmapData.Height];

            for (var y = 0; y < bitmapData.Height; y++)
            {
                byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                for (var x = 0; x < bitmapData.Width; x++)
                {
                    var r = row[x * pixelSize + 0];
                    var g = row[x * pixelSize + 1];
                    var b = row[x * pixelSize + 2];
                    var pix = BitConverter.ToUInt32(new byte[] { 0, r, g, b }, 0);
                    res[x, y] = pix;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return res;
        }
    }
}
