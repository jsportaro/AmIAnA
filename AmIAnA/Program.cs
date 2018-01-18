using AmIAnA.AI;
using AmIAnA.Imaging;
using AmIAnA.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA
{
    

    class Program
    {
        public static NeuralNetwork neuralNetwork = new NeuralNetwork();

        static void Main(string[] args)
        {
            //neuralNetwork.Train();

            var action = Console.ReadLine();

            if (action == "train")
            {
                var serializedExamples = ReadTrainingSet(ConfigurationManager.AppSettings["trainingImage"]);
            }
            else
            {
                Application.Run(new ImageInput());
            }
            Console.ReadKey();
        }

        private static IEnumerable<Sample>[] ReadTrainingSet(string trainingSetDirectory)
        {
            var viewer = new ImageViewer();
            Task.Run(() => { Application.Run(viewer); viewer.Show(); });
            var ledgerPath = Path.Combine(trainingSetDirectory, "ledger.txt");

            return File.ReadAllLines(ledgerPath)
                .Select(l =>
                {
                    using (var example = new ImageSampler(l))
                    {

                        example.DrawToViewer(viewer);
                        var samples = example.GetSamples();

                        return samples;
                    }
                })
                .ToArray();
        }
    }
}
