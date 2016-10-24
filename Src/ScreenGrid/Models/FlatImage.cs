namespace ScreenGrid.Models
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

        public UInt32[] GetVerticalStripe(int ix)
        {
            var result = new UInt32[this.Height];
            for (var iy = 0; iy < this.Height; iy++)
            {
                result[iy] = this.pixels[ix, iy];
            }

            return result;
        }

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

        private IList<IntegerSegment> FindHorizontalZeroSegments(int iy, int minimalSegmentLength)
        {
            return IntegerSegmentUtils.FindZeroSegments(FlatImage.GetDerivative(this.GetHorizontalStripe(iy)), minimalSegmentLength);
        }

        private IList<IntegerSegment> FindVerticalZeroSegments(int ix, int minimalSegmentLength)
        {
            return IntegerSegmentUtils.FindZeroSegments(FlatImage.GetDerivative(this.GetVerticalStripe(ix)), minimalSegmentLength);
        }

        public Models.Geometry.Rectangle FindBoundsOfInnerImage()
        {
            // TODO: implement more robust algorithm
            // TODO: add more tests for this method
            const int minimalSegmentLength = 8;

            var stripeCH = IntegerSegmentUtils.FindZeroSegments(FlatImage.GetDerivative(this.GetHorizontalStripe(this.Height / 2)), minimalSegmentLength);
            var stripeCV = IntegerSegmentUtils.FindZeroSegments(FlatImage.GetDerivative(this.GetVerticalStripe(this.Width / 2)), minimalSegmentLength);

            // TODO: optimize code
            for (var iy = 1 * this.Height / 4; iy < 3 * this.Height / 4; iy += minimalSegmentLength)
            {
                stripeCH = IntegerSegmentUtils.IntersectionOfSegments(new[]
                    { 
                        stripeCH, 
                        FindHorizontalZeroSegments(iy, minimalSegmentLength)
                    });
            }

            for (var ix = 1 * this.Width / 4; ix < 3 * this.Width / 4; ix += minimalSegmentLength)
            {
                stripeCV = IntegerSegmentUtils.IntersectionOfSegments(new[]
                    { 
                        stripeCV, 
                        FindVerticalZeroSegments(ix, minimalSegmentLength)
                    });
            }

            if ((stripeCH.Count > 1) && (stripeCV.Count > 1))
            {
                var maxH = IntegerSegmentUtils.SegmentsWithMaxDistance(stripeCH);
                var maxV = IntegerSegmentUtils.SegmentsWithMaxDistance(stripeCV);

                return new Geometry.Rectangle
                {
                    X = maxH.Start + 1,
                    Y = maxV.Start + 1,
                    Width = maxH.End - maxH.Start - 2,
                    Height = maxV.End - maxV.Start - 2,
                };
            }
            else
            {
                return new Geometry.Rectangle();
            }
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

        public unsafe System.Drawing.Bitmap ToBitmap()
        {
            var bitmap = new System.Drawing.Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            const int pixelSize = 3;
            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            for (var y = 0; y < bitmapData.Height; y++)
            {
                var row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                for (var x = 0; x < bitmapData.Width; x++)
                {
                    var pix = BitConverter.GetBytes(this.pixels[x, y]);
                    row[x * pixelSize + 0] = pix[1];
                    row[x * pixelSize + 1] = pix[2];
                    row[x * pixelSize + 2] = pix[3];
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }
    }
}
