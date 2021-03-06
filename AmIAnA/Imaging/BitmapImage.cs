﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Imaging
{
    public class BitmapImage : IAddressableImage
    {
        private Bitmap bitmap;

        public int Width       => bitmap.Width;
        public int Height      => bitmap.Height;
        public Image BaseImage => bitmap;

        public BitmapImage(string filename)
            : this(new Bitmap(filename))
        {
            
        }

        public BitmapImage(Bitmap bitmap)
        {
            this.bitmap = bitmap.Clone() as Bitmap;
        }

        public Color GetPixel(int x, int y)
        {
            return bitmap.GetPixel(x, y);
        }

        public bool MeetsThreshold(Color color)
        {
            var result = color.Name != "White";

            var anotherTest = color.R != Color.White.R && color.B != Color.White.B && color.G != Color.White.G;

            return anotherTest;
        }

        public void Dispose()
        {
            bitmap?.Dispose();
        }
    }
}
