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

            var flatImage = LoadFlatImageFromResource("ScreenGrid.Models.Tests.Resources.ImageSimple.png");

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
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Models.Tests.Resources.ImageSimple.png");
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
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Models.Tests.Resources.ImageSimple.png");

            const int minimalSegmentLength = 8;
            var step = flatImage.Height / 8;

            var list = new List<Tuple<int, int>>();
            for (var iy = minimalSegmentLength + step; iy < flatImage.Height - (minimalSegmentLength + step); iy += step)
            {
                var stripe = flatImage.GetHorizontalStripe(iy);
                var derivative = FlatImage.GetDerivative(stripe);
                var segments = FlatImage.FindZeroSegments(derivative, minimalSegmentLength);

                Assert.AreEqual(2, segments.Count, String.Format("iy={0}", iy));
            }
        }

        [Test]
        public void FindBoundingsOfInnerImage()
        {
            var flatImage = LoadFlatImageFromResource("ScreenGrid.Models.Tests.Resources.ImageSimple.png");

            var result = flatImage.FindBoundingsOfInnerImage();

            Assert.AreEqual(12, result.X);
            Assert.AreEqual(10, result.Y);
            Assert.AreEqual(40, result.Width);
            Assert.AreEqual(28, result.Height);
        }

        [Test]
        public void FindZeroSegmentsTest1()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0, 1, 2, 1, 0, 3, 4, 0, 3, 5 };
            var result = FlatImage.FindZeroSegments(array, 4);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Item1);
            Assert.AreEqual(5, result[0].Item2);
        }

        [Test]
        public void FindZeroSegmentsTestThreeSegments()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0, 1, 2, 1, 0, 0, 3, 4, 0, 0, 3, 0, 5 };
            var result = FlatImage.FindZeroSegments(array, 2);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Item1);
            Assert.AreEqual(5, result[0].Item2);
            Assert.AreEqual(9, result[1].Item1);
            Assert.AreEqual(10, result[1].Item2);
            Assert.AreEqual(13, result[2].Item1);
            Assert.AreEqual(14, result[2].Item2);
        }

        [Test]
        public void FindZeroSegmentsTestNoSegments()
        {
            var array = new UInt32[] { 1, 0, 0, 1, 2, 3 };
            var result = FlatImage.FindZeroSegments(array, 3);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindZeroSegmentsTestShortAllZeroSegment()
        {
            var array = new UInt32[] { 0, 0, 0, 0, };
            var result = FlatImage.FindZeroSegments(array, 4);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Item1);
            Assert.AreEqual(3, result[0].Item2);
        }

        [Test]
        public void FindZeroSegmentsTestAllZeroSegment()
        {
            var array = new UInt32[] { 0, 0, 0, 0, 0, 0 };
            var result = FlatImage.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Item1);
            Assert.AreEqual(5, result[0].Item2);
        }

        [Test]
        public void FindZeroSegmentsTestSimpleSegment1()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0 };
            var result = FlatImage.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Item1);
            Assert.AreEqual(5, result[0].Item2);
        }

        [Test]
        public void FindZeroSegmentsTestSimpleSegment2()
        {
            var array = new UInt32[] { 0, 0, 0, 0, 0, 1 };
            var result = FlatImage.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Item1);
            Assert.AreEqual(4, result[0].Item2);
        }

        [Test]
        public void IntersectionOfSegmentsFullIntersection()
        {
            var result = FlatImage.IntersectionOfSegments(
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(1, 2));
            Assert.AreEqual(new Tuple<int, int>(1, 2), result);
        }

        [Test]
        public void IntersectionOfSegmentsPartialIntersection()
        {
            var a = new Tuple<int, int>(1, 4);
            var b = new Tuple<int, int>(2, 8);

            Assert.AreEqual(new Tuple<int, int>(2, 4), FlatImage.IntersectionOfSegments(a, b));
            Assert.AreEqual(new Tuple<int, int>(2, 4), FlatImage.IntersectionOfSegments(b, a));
        }

        [Test]
        public void IntersectionOfSegmentsNoIntersection()
        {
            var result = FlatImage.IntersectionOfSegments(
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(4, 5));
            Assert.IsNull(result);
        }

        [Test]
        public void IntersectionOfSegments()
        {
            var result = FlatImage.IntersectionOfSegments(new[]
                {
                    new[] { new Tuple<int, int>(1, 2), new Tuple<int, int>(4, 6) },
                    new[] { new Tuple<int, int>(1, 2), new Tuple<int, int>(5, 10) },
                });

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new Tuple<int, int>(1, 2), result[0]);
            Assert.AreEqual(new Tuple<int, int>(5, 6), result[1]);
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
