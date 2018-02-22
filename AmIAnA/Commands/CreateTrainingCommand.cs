using AmIAnA.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA.Commands
{
    class CreateTrainingCommand : ICommand
    {
        private bool prettyPictures;

        public bool Execute(string command, State state)
        {
            var encodedCommand = command.ToUpper();

            var parts = command.Split(new char[] { ' ' });

            if (parts[0] != "C")
            {
                return false;
            }

            prettyPictures = parts.Length == 2 ? !(parts[1] == "NOSHOW") : true;

            var serializedExamples = ReadTrainingSet(state.GetTrainingLedgerPath);
            var trainingDataPath = state.GetTrainingDataPath;

            if (File.Exists(trainingDataPath))
                File.Delete(trainingDataPath);

            var lines = serializedExamples.Select(se =>
            {
                var resultPair = se.IsAnA ? "1, 0" : "0, 1";
                var line = $"{resultPair}, {string.Join(", ", se.GetSamples().Select(s => s.NormalizedLuminosity))}";

                return line;
            });

            File.WriteAllLines(trainingDataPath, lines);

            Console.WriteLine($"Training data written to {trainingDataPath}");

            return true;
        }

        public string GetHelp()
        {
            return "(C)reate command takes the images in the Data file and creates the vectors used to train the ANN." + Environment.NewLine + 
                   "    By default, you can watch the program creating the vectors.  It looks neat.  If you don't want to" + Environment.NewLine + 
                   "    see this then pass NOSHOW.  For example  'C NOSHOW'";
        }

        private IEnumerable<ImageSampler> ReadTrainingSet(string trainingLedgerPath)
        {
            var viewer = new ImageViewer();

            if (prettyPictures)
                Task.Run(() =>
                {
                    viewer.Show();
                    Application.Run(viewer);
                });
            else
                Console.WriteLine("This may take a while... ");

            var count = 1;
            var updateAt = 25;
            var lines = File.ReadAllLines(trainingLedgerPath);
            var samplesList = new List<ImageSampler>();

            for (var i = 0; i < lines.Count(); i++)
            {
                using (var example = new ImageSampler(lines[i]))
                {
                    if (prettyPictures)
                        example.DrawToViewer(viewer);
                    else if (count % updateAt == 0)
                        Console.WriteLine($"Finished {count}...");

                    samplesList.Add(example);
                }
            }

            return samplesList.ToArray();
        }
    }
}
