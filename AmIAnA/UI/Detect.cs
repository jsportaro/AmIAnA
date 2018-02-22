using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmIAnA.AI;
using AmIAnA.Imaging;

namespace AmIAnA.UI
{
    public partial class Detect : Form
    {
        BlobFinder blobFinder;
        BackPropNeuralNet bnn;
        bool repaint = true;
        
        public Detect(BackPropNeuralNet bnn)
        {
            InitializeComponent();

            DoubleBuffered = true;
            Dock = DockStyle.Fill;

            blobFinder = new BlobFinder();
            this.bnn = bnn;


            this.panel1.Paint += new PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new MouseEventHandler(this.panel1_MouseUp);
        }

        private Point _origin = Point.Empty;
        private Point _terminus = Point.Empty;
        private Boolean _draw = false;
        private List<Tuple<Point, Point>> _lines = new List<Tuple<Point, Point>>();

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _draw = true;
                _origin = e.Location;
            }
            else
            {
                _draw = false;
                _origin = Point.Empty;
            }

            _terminus = Point.Empty;
            if (repaint)
                Invalidate();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_draw && !_origin.IsEmpty && !_terminus.IsEmpty)
                _lines.Add(new Tuple<Point, Point>(_origin, _terminus));
            _draw = false;
            _origin = Point.Empty;
            _terminus = Point.Empty;

            if (repaint)
                Refresh();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _terminus = e.Location;
            if (repaint)
                Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var line in _lines)
            {
                var pen = new Pen(Brushes.Black, 10F);
                e.Graphics.DrawLine(pen, line.Item1, line.Item2);
            }

            if (!_origin.IsEmpty && !_terminus.IsEmpty)
                e.Graphics.DrawLine(Pens.Red, _origin, _terminus);
        }

        private Bitmap PanelToBitmap()
        {
            var bmp = new Bitmap(panel1.Width, panel1.Height);
            //DrawToBitmap(bmp, new Rectangle(panel1.Location.X, panel1.Location.Y, bmp.Width, bmp.Height));
            panel1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            return bmp;
        }

        private void runNeuralNet_Click(object sender, EventArgs e)
        {
            reset.Enabled = true;

            var bmp = new BitmapImage(PanelToBitmap());
            var blobs = blobFinder.From(bmp);
            if (blobs.Any())
            {
                panel1.CreateGraphics().DrawRectangles(Pens.Red, blobs.ToArray());

                var imageSampler = new ImageSampler();
                var samples = imageSampler.SampleImage(bmp).GetSamples().Select(s => s.NormalizedLuminosity).ToArray();
                var output = bnn.ComputeOutputs(samples);

                if (output[0] > 0.90 && output[1] < 0.05)
                {
                    BackColor = Color.Green;
                }
                else
                {
                    BackColor = Color.Red;
                }
            }
            repaint = false;

        }

        private void reset_Click(object sender, EventArgs e)
        {
            reset.Enabled = false;
            repaint = true;

            _lines.Clear();
            BackColor = SystemColors.Control;
            Refresh();
        }
    }
}

