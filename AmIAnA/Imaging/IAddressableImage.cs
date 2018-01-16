using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Imaging
{
    public interface IAddressableImage
    {
        int Width { get; }

        int Height { get; }

        Image BaseImage { get; }

        Color GetPixel(int x, int y);
    }
}
