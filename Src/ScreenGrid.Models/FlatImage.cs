namespace ScreenGrid.Models
{
    using System;

    /// <summary>
    /// 'Flattened' image representation for fast processing
    /// </summary>
    public class FlatImage
    {
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
                throw new FormatException(String.Format("Image format {0} not supported", bitmap.PixelFormat));
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
