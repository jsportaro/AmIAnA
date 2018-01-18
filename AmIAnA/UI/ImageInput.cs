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
    public partial class ImageInput : Form
    {
        public ImageInput()
        {
            InitializeComponent();
        }

        private void ImageInput_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
