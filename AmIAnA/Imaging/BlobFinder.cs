using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Imaging
{
    public class BlobFinder
    {
        int currentLabel = 1;

        List<LabeledPixel> labeledPixels = new List<LabeledPixel>();
        int[,] labels;
        Queue<LabeledPixel> queue = new Queue<LabeledPixel>();
        IAddressableImage Image;

        List<Rectangle> boundedBoxes = new List<Rectangle>();

        public IEnumerable<Rectangle> From(IAddressableImage image)
        {
            labels = new int[image.Width, image.Height];

            var rectangles = new List<Rectangle>();
            Image = image;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (Image.MeetsThreshold(Image.GetPixel(x, y)) 
                        && labels[x, y] == 0 )
                    {
                        var found = new LabeledPixel(x, y, currentLabel);
                        labels[x, y] = currentLabel;
                        if (!queue.Contains(found))
                            queue.Enqueue(found);
                        ProcessQueue(x, y);
                    }
                }
            }

            return CreateRectangle();
        }

        private IEnumerable<Rectangle> CreateRectangle()
        {
           
            Dictionary<int, TempRectangle> rectangles = new Dictionary<int, TempRectangle>();
            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    if (labels[x, y] > 0)
                    {
                        if (!rectangles.Keys.Contains(labels[x, y]))
                        {
                            var rectangle = new TempRectangle()
                            {
                                MinX = x,
                                MaxX = x,
                                MinY = y,
                                MaxY = y
                            };
                            rectangles[labels[x, y]] = rectangle;
                        }
                        else
                        {
                            var rectangle = rectangles[labels[x, y]];

                            if (rectangle.MinX > x)
                                rectangle.MinX = x;
                            if (rectangle.MinY > y)
                                rectangle.MinY = y;
                            if (rectangle.MaxY < y)
                                rectangle.MaxY = y;
                            if (rectangle.MaxX < x)
                                rectangle.MaxX = x;
                        }
                        
                    }
                }
            }

            return rectangles.Select(r => new Rectangle(r.Value.MinX, r.Value.MinY, r.Value.MaxX - r.Value.MinX, r.Value.MaxY - r.Value.MinY));
        }

        private void ProcessQueue(int pivotX, int pivotY)
        {
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
                            if (Image.MeetsThreshold(Image.GetPixel(neighbor.X, neighbor.Y))
                                && labels[neighbor.X, neighbor.Y] == 0 )
                            {
                                var connectedNeighbor = new LabeledPixel(neighbor.X, neighbor.Y, currentLabel);

                                if (!labeledPixels.Contains(connectedNeighbor))
                                    queue.Enqueue(connectedNeighbor);
                                labels[neighbor.X, neighbor.Y] = currentLabel;
                            }
                        }
                    }
                }

            }
            while (queue.Any());
            currentLabel++;
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

        public class TempRectangle
        {
            public int MinX { get; set; }
            public int MaxX { get; set; }
            public int MinY { get; set; }
            public int MaxY { get; set; }
        }
    }
}
