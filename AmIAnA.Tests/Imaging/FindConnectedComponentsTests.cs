using AmIAnA.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AmIAnA.Tests.Imaging
{
    public class FindConnectedComponentsTests
    {
        [Fact]
        public void Able_To_Find_One_Rectangle()
        {
            int[,] imageData = new int[10, 10]
            {
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            var finder = new BlobFinder();

            var rectangles = finder.From(new TestImage(imageData));

            Assert.True(rectangles.Count() == 1);
            Assert.NotEmpty(rectangles);
        }

        [Fact]
        public void Able_To_Find_Two_Rectangle()
        {
            int[,] imageData = new int[10, 10]
            {
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            var finder = new BlobFinder();

            var rectangles = finder.From(new TestImage(imageData));

            Assert.True(rectangles.Count() == 2);
            Assert.NotEmpty(rectangles);
        }


        private class TestImage : IAddressableImage
        {
            private int[,] imageData;

            public TestImage(int[,] imageData)
            {
                this.imageData = imageData;
            }

            public int Width => 10;

            public int Height => 10;

            public Image BaseImage => throw new NotSupportedException();

            public void Dispose()
            {
            }

            public Color GetPixel(int x, int y)
            {
                return imageData[y, x] == 1 ? Color.Black : Color.White;
            }

            public bool MeetsThreshold(Color color)
            {
                var result = color.Name != "White";

                var anotherTest = color.R != Color.White.R && color.B != Color.White.B && color.G != Color.White.G;

                return anotherTest;
            }
        }
    }
}
