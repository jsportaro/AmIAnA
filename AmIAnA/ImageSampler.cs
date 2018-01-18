using AmIAnA.Imaging;
using AmIAnA.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA
{
    public class ImageSampler : IDisposable
    {
        private const int Rows = 5;
        private const int Columns = 5;

        public bool IsAnA { get; set; }

        public TimeSpan TimeToFindLetter { get; private set; }

        private readonly Sample[,] samples = new Sample[Columns, Rows];
        private readonly IEnumerable<Rectangle> rectangles;
        private readonly BitmapImage image;

        public ImageSampler(string serializedExample)
        {
            var parts = serializedExample.Split(new char[] { ',' });
            var pathToImage = parts[2];

            image = new BitmapImage(new Bitmap(pathToImage));
            rectangles = GetRectangles(image);

            foreach (var rec in rectangles)
            {
                var width = (int)Math.Round((double)rec.Width / Columns, 0, MidpointRounding.AwayFromZero);
                var height = (int)Math.Round((double)rec.Height / Rows, 0, MidpointRounding.AwayFromZero);

                double halfFill = (width * height) / 2;

                int count = 1;

                for (int y = 0 + rec.Top, row = 0; y < rec.Bottom && row < Rows; y += height, row++)
                {
                    for (int x = 0 + rec.Left, column = 0; x < rec.Right && column < Columns; x += width, column++)
                    {
                        var newSampleArea = new Rectangle(x, y, width, height);
                        var newSample = new Sample() { BoundedRectangle = newSampleArea, Count = count };
                        count++;
                        for (int innerX = x; innerX < x + width; innerX++)
                        {
                            for (int innerY = y; innerY < y + height; innerY++)
                            {
                                if (image.MeetsThreshold(image.GetPixel(innerX, innerY)))
                                    newSample.Pixels++;
                            }
                        }


                        samples[column, row] = newSample;
                        var test = samples[column, row].NormalizedLuminosity;
                    }
                }
            }

            IsAnA = parts[0].Trim() == "1";
        }

        public void DrawToViewer(ImageViewer viewer)
        {
            viewer.SetImage(image);
            foreach (var rec in rectangles)
            {
                viewer.WriteRectangle(rec);
            }

            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    var sample = samples[column, row];
                    viewer.WriteRectangle(sample.BoundedRectangle);
                    viewer.WriteNormalizedResult(column, row, sample);
                }
            }

            viewer.WriteTimeToFindLetter(TimeToFindLetter);
        }

        public IEnumerable<Sample> GetSamples()
        {
            return samples.Cast<Sample>().ToArray();
        }

        private IEnumerable<Rectangle> GetRectangles(IAddressableImage image)
        {
            var cc = new BlobFinder();

            var sw = new Stopwatch();
            sw.Start();
            var rectangles = cc.From(image);
            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            TimeToFindLetter = sw.Elapsed;

            return rectangles;
        }

        public void Dispose()
        {
            image?.Dispose();
        }
    }
}
