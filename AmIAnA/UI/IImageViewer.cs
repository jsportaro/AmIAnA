using AmIAnA.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.UI
{
    public interface IImageViewer
    {
        void SetImage(IAddressableImage image);
        void WriteRectangle(Rectangle rectangle);
        void WriteNormalizedResult(int x, int y, Sample sample);
        void WriteTimeToFindLetter(TimeSpan timeToFind);
    }
}
