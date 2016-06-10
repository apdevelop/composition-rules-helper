namespace ScreenGrid.Models.Tests
{
    using NUnit.Framework;
    using System.Drawing;

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
