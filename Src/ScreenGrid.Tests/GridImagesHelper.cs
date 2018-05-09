using NUnit.Framework;
using ScreenGrid.Models.Grids;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace ScreenGrid.Tests
{
    /// <summary>
    /// Generating bitmap (PNG) icons for grid selection menu
    /// </summary>
    [TestFixture]
    class GridImagesHelper
    {
        private const int ImageWidth = 64;
        private const int ImageHeight = 48;

        [Test]
        public void RenderGridImagesAsPngFiles()
        {
            foreach (GridType gridType in Enum.GetValues(typeof(GridType)))
            {
                RenderGridImageAsPngFile(gridType);
            }
        }

        private static void RenderGridImageAsPngFile(GridType gridType)
        {
            var gridLines = GridCreator.CreateGrid(gridType, ImageWidth, ImageHeight, false);

            var outputPath = Path.Combine(Path.GetTempPath(), "ScreenGrid");
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            using (var image = new Bitmap(ImageWidth, ImageHeight))
            {
                using (var g = Graphics.FromImage(image))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    using (var pen = new Pen(Color.Black))
                    {
                        foreach (var line in gridLines)
                        {
                            g.DrawLine(
                                pen,
                                new Point((int)(line.p1.X * (ImageWidth - 1)), (int)(line.p1.Y * (ImageHeight - 1))),
                                new Point((int)(line.p2.X * (ImageWidth - 1)), (int)(line.p2.Y * (ImageHeight - 1))));
                        }
                    }
                }

                image.Save(Path.Combine(outputPath, gridType.ToString() + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        [Test]
        public void RenderGridImagesAsResourceDictionaryXamlPaths()
        {
            var lines = new List<string>();
            foreach (GridType gridType in Enum.GetValues(typeof(GridType)))
            {
                if (gridType != GridType.None)
                {
                    var key = "pathDataGridType" + gridType.ToString();
                    var path = RenderGridImageAsXamlPath(gridType);

                    lines.Add(String.Empty);
                    lines.Add("<sys:String x:Key=\"" + key + "\">");
                    lines.Add(path);
                    lines.Add("</sys:String>");
                }
            }

            File.WriteAllLines(Path.Combine(Path.GetTempPath(), "Grids.xaml"), lines);
        }

        private static string RenderGridImageAsXamlPath(GridType gridType)
        {
            var gridLines = GridCreator.CreateGrid(gridType, ImageWidth, ImageHeight, false);

            var result = new StringBuilder();
            foreach (var line in gridLines)
            {
                var command = String.Format(CultureInfo.InvariantCulture, "M{0:F4},{1:F4}L{2:F4},{3:F4}", line.p1.X, line.p1.Y, line.p2.X, line.p2.Y);
                result.Append(command);
            }

            return result.ToString();
        }
    }
}
