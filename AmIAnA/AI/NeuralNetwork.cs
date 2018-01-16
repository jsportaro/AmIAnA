using AmIAnA.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA.AI
{
    public class NeuralNetwork
    {
        public void Train(string directoryOfImages)
        {
            if (string.IsNullOrWhiteSpace(directoryOfImages))
                throw new ArgumentException(nameof(directoryOfImages));

            var ledgerPath = Path.Combine(directoryOfImages, "ledger.txt");

            if (!Directory.Exists(directoryOfImages) || !File.Exists(ledgerPath))
                throw new InvalidOperationException("Can't train if nothing is there to train");

        }

        
    }
}
