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
using static AmIAnA.Program;

namespace AmIAnA.UI
{
    public partial class ImageViewer : Form, IImageViewer
    {
        Label[,] labels;

        public ImageViewer()
        {
            InitializeComponent();

            labels = new Label[5, 5]
            {
                { one, two, three, four, five },
                { six, seven, eight, nine, ten },
                {eleven, twelve, thirteen, fourteen, fifteen },
                { sixteen, seventeen, eighteen, nineteen, twenty},
                {twentyone, twentytwo, twentythree, twentyfour, twentyfive }
            };
        }

        public void SetImage(IAddressableImage image)
        {
            this.image.Image = image.BaseImage.Clone() as Image;
        }

        public void WriteRectangle(Rectangle rectangle)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                image.CreateGraphics().DrawRectangle(pen, rectangle);
            }

        }

        public void WriteLine(Point from, Point to)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                image.CreateGraphics().DrawLine(pen, from, to);
            }

        }

        public void WriteNormalizedResult(int x, int y, Sample sample)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labels[y, x].Text = sample.NormalizedLuminosity.ToString("F2");
            });
        }

        public void WriteTimeToFindLetter(TimeSpan timeToFind)
        {
            this.Invoke((MethodInvoker)delegate
            {
                timeToFindText.Text = timeToFind.Milliseconds.ToString() + " ms";
            });
        }
    }
}
