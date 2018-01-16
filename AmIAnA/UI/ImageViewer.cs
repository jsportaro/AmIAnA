using AmIAnA.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA.UI
{
    public partial class ImageViewer : Form
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        public void SetImage(IAddressableImage image)
        {
            this.image.Image = image.BaseImage;
        }

        public void WriteRectangle(Rectangle rectangle)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                image.CreateGraphics().DrawRectangle(pen, rectangle);
                // e.Graphics.DrawRectangle(pen, ee);
            }

        }
    }
}
