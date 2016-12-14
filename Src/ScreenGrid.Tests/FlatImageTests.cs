namespace ScreenGrid.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using NUnit.Framework;

    [TestFixture]
    public class FlatImageTests
    {
        [Test]
        public void GetHorizontalStripeTest()
        {
            const int width = 64;
            const int height = 48;

            var flatImage = LoadFlatImageFromResource("ScreenGrid.Tests.Resources.ImageSimple.png");

            Assert.AreEqual(width, flatImage.Width);
            Assert.AreEqual(height, flatImage.Height);

            var stripeTop = flatImage.GetHorizontalStripe(0);
            Assert.AreEqual(width, stripeTop.Length);

            var stripeMiddle = flatImage.GetHorizontalStripe(height / 2);
            Assert.AreEqual(width, stripeMiddle.Length);
        }

        [Test]
        public void GetDerivativeTest()
        {
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Tests.Resources.ImageSimple.png");
            var stripeMiddle = flatImage.GetHorizontalStripe(flatImage.Height / 2);

            var dMiddle = FlatImage.GetDerivative(stripeMiddle);
            Assert.AreEqual(stripeMiddle.Length, dMiddle.Length);

            // Border detection
            Assert.AreEqual(0, dMiddle[11]);
            Assert.Greater(dMiddle[12], 0);
        }

        [Test]
        public void FindBordersTest()
        {
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Tests.Resources.ImageSimple.png");

            const int minimalSegmentLength = 8;
            var step = flatImage.Height / 8;

            var list = new List<Tuple<int, int>>();
            for (var iy = minimalSegmentLength + step; iy < flatImage.Height - (minimalSegmentLength + step); iy += step)
            {
                var stripe = flatImage.GetHorizontalStripe(iy);
                var derivative = FlatImage.GetDerivative(stripe);
                var segments = IntegerSegmentUtils.FindZeroSegments(derivative, minimalSegmentLength);

                Assert.AreEqual(2, segments.Count, String.Format("iy={0}", iy));
            }
        }

        [Test]
        public void FindBoundsOfInnerImage()
        {
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Tests.Resources.ImageSimple.png");

            var result = flatImage.FindBoundsOfInnerImage();

            Assert.AreEqual(12, result.X);
            Assert.AreEqual(10, result.Y);
            Assert.AreEqual(40, result.Width);
            Assert.AreEqual(28, result.Height);
        }

        private static FlatImage LoadFlatImageFromResource(string resourceName)
        {
            FlatImage flatImage;

            var imageData = ReadResource(resourceName);

            using (var ms = new System.IO.MemoryStream(imageData))
            {
                using (var original = new Bitmap(ms))
                {
                    using (var bitmap = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            g.DrawImage(original, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                        }

                        flatImage = new FlatImage(bitmap);
                    }
                }
            }

            return flatImage;
        }

        private static byte[] ReadResource(string resourceName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            byte[] data = null;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
            }

            return data;
        }
    }
}
