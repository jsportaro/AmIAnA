using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Imaging
{
    public class FindConnectedComponent
    {
        int currentLabel = 1;

        List<LabeledPixel> labeledPixels = new List<LabeledPixel>();
        Queue<LabeledPixel> queue = new Queue<LabeledPixel>();
        IAddressableImage Image;

        List<Rectangle> boundedBoxes = new List<Rectangle>();

        public IEnumerable<Rectangle> From(IAddressableImage image)
        {
            var rectangles = new List<Rectangle>();
            Image = image;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (MeetsThreshold(Image.GetPixel(x, y)) 
                        && !labeledPixels.Any(l => l.X == x && l.Y == y))
                    {
                        var found = new LabeledPixel(x, y, currentLabel);
                        labeledPixels.Add(found);
                        if (!queue.Contains(found))
                            queue.Enqueue(found);
                        
                        ProcessQueue(x, y);
                        
                    }
                }
            }
            
            return labeledPixels
                .GroupBy(l => l.Label)
                .Select(l => CreateRectangle(l))
                .AsEnumerable();
        }

        private Rectangle CreateRectangle(IGrouping<int, LabeledPixel> group)
        {
            var xs = group.OrderBy(l => l.X);
            var ys = group.OrderBy(l => l.Y);

            return new Rectangle(xs.First().X, ys.First().Y, xs.Last().X - xs.First().X, ys.Last().Y - ys.First().Y);
        }

        private void ProcessQueue(int pivotX, int pivotY)
        {
            var boundedBox = new Rectangle();


            do
            {
               
                var found = queue.Dequeue();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        var neighbor = new Point(found.X + x, found.Y + y);

                        if ((neighbor.X >= 0 && neighbor.Y >= 0) &&
                            (neighbor.X < Image.Width && neighbor.Y < Image.Height))
                        {
                            if (MeetsThreshold(Image.GetPixel(neighbor.X, neighbor.Y))
                                && !labeledPixels.Any(l => l.X == neighbor.X && l.Y == neighbor.Y))
                            {
                                var connectedNeighbor = new LabeledPixel(neighbor.X, neighbor.Y, currentLabel);

                                if (!labeledPixels.Contains(connectedNeighbor))
                                    queue.Enqueue(connectedNeighbor);

                                labeledPixels.Add(connectedNeighbor);
                            }
                        }
                    }
                }
                
            }
            while (queue.Any());

            currentLabel++;
        }

        private bool MeetsThreshold(Color color)
        {
            var result = color.Name != "White";

            var anotherTest = color.R != Color.White.R && color.B != Color.White.B && color.G != Color.White.G;

            return anotherTest;
        }

        private class LabeledPixel
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Label { get; set; }

            public LabeledPixel(int x, int y, int label)
            {
                X = x;
                Y = y;
                Label = label;
            }
        }
    }
}
