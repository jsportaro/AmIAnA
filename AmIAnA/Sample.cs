using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA
{
    public class Sample
    {
        public Rectangle BoundedRectangle { get; set; }
        public int Pixels { get; set; }
        public int Count { get; set; }
        public double NormalizedLuminosity
        {
            get
            {
                var a = -10;
                var b = 10;

                return ((b - a) * ((double)Pixels / (BoundedRectangle.Height * BoundedRectangle.Width))) + (a);
            }
        }
    }
}
