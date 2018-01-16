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

            var serializedExamples = ReadTrainingSet(ConfigurationManager.AppSettings["trainingImage"]);


            Console.ReadKey();
        }

        private static Example[] ReadTrainingSet(string trainingSetDirectory)
        {
            var viewer = new ImageViewer();
            Task.Run(() => { Application.Run(viewer); viewer.Show(); });
            var ledgerPath = Path.Combine(trainingSetDirectory, "ledger.txt");

            return File.ReadAllLines(ledgerPath)
                .Select(l => new Example(l, viewer))
                .ToArray();
        }

        private class Example
        {
            public bool IsAnA { get; set; }

            public Example(string serializedExample, ImageViewer viewer)
            {
                var parts = serializedExample.Split(new char[] { ',' });
                var pathToImage = parts[2];

                using (var image = new Bitmap(pathToImage))
                {
                    var cc = new FindConnectedComponent();
                    var wrappedImage = new BitmapImage(image);

                    var sw = new Stopwatch();
                    sw.Start();
                    var rectangles = cc.From(wrappedImage);
                    sw.Stop();
                    Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
                    viewer.SetImage(wrappedImage);


                    foreach (var rec in rectangles)
                    {
                        viewer.WriteRectangle(rec);
                    }
                }

                IsAnA = parts[0].Trim() == "1";
            }
        }
    }
}
