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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA
{
    

    class Program
    {
        //public static NeuralNetwork neuralNetwork = new NeuralNetwork();
        private static bool prettyPictures = true;
        private static NeuralNetwork neuralNetwork;
        private static BackPropNeuralNet bnn;

        static void Main(string[] args)
        {
            neuralNetwork = new NeuralNetwork(25, 100, 2);

            PrintHelp();
            while (true)
            {
                var action = Console.ReadLine().ToUpper();

                if (action == "C")
                {
                    var serializedExamples = ReadTrainingSet(ConfigurationManager.AppSettings["trainingImage"]);
                    var trainingDataPath = GetTrainingDataPath();

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
                }
                else if (action == "T")
                {
                    var path = GetWeightDataPath();
                    //int maxEpochs = 1000;
                    //double learnRate = 0.05;
                    //double momentum = 0.01;
                    //Console.WriteLine("Setting maxEpochs = " + maxEpochs);
                    //Console.WriteLine("Setting learnRate = " + learnRate.ToString("F2"));
                    //Console.WriteLine("Setting momentum  = " + momentum.ToString("F2"));

                    //Console.WriteLine("Starting training");
                    var trainingData = File.ReadAllLines(GetTrainingDataPath()).Select(l =>
                    {
                        var raw = l.Split(new char[] { ',' }).Select(i => Double.Parse(i));
                        return raw.ToArray();
                    }).ToArray();

                    //neuralNetwork.Train(trainingData, maxEpochs, learnRate, momentum);
                    var rnd = new Random(1);
                    int numInput = 25;
                    int numHidden = 18;
                    int numOutput = 2;
                    int numWeights = (numInput * numHidden) + (numHidden * numOutput) + (numHidden + numOutput);

                    double[] xValues = new double[numInput];
                    double[] yValues; // outputs
                    double[] tValues = new double[numOutput];

                    Console.WriteLine($"Creating a {numInput}-input, {numHidden}-hidden, {numOutput}-output neural network");
                    Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
                    Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");

                    bnn = new BackPropNeuralNet(numInput, numHidden, numOutput);

                    Console.WriteLine("\nGenerating random initial weights and bias values");
                    double[] initWeights = new double[numWeights];
                    for (int i = 0; i < initWeights.Length; ++i)
                        initWeights[i] = (0.1 - 0.01) * rnd.NextDouble() + 0.01;

                    Console.WriteLine("Loading neural network initial weights and biases into neural network");
                    bnn.SetWeights(initWeights);

                    double learnRate = 0.8;  // learning rate - controls the maginitude of the increase in the change in weights.
                    double momentum = 0.2; // momentum - to discourage oscillation.
                    Console.WriteLine($"Setting learning rate = {learnRate.ToString("F2")} and momentum = {momentum.ToString("F2")}");

                    var maxEpochs = 1000;
                    var errorThresh = 0.00001;
                    Console.WriteLine($"Setting max epochs = {maxEpochs} and error threshold = {errorThresh.ToString("F6")}");


                    var epoch = 0;
                    var error = double.MaxValue;
                    Console.WriteLine("\nBeginning training using back-propagation\n");
                    double[] weights = bnn.Train(trainingData, maxEpochs, learnRate, momentum);

                    File.WriteAllText(path, string.Join(",", weights));

                }
                else if (action == "VISUALS ON" || action == "VI")
                {
                    prettyPictures = true;
                    Console.WriteLine("Pretty pictures ahoy!");

                }
                else if (action == "VISUALS OFF" || action == "VO")
                {
                    prettyPictures = false;
                    Console.WriteLine("Nothing pretty");
                }

                else if (action == "Q")
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"Didn't recognize {action}.  Only allows (C),");
                }

                PrintPrompt();
            }
        }

        private static string GetTrainingDataPath()
        {
            return Path.Combine(GetExePath(), ConfigurationManager.AppSettings["trainingData"]);        
        }

        private static string GetWeightDataPath()
        {
            return Path.Combine(GetExePath(), ConfigurationManager.AppSettings["weightData"]);
        }

        private static void Shuffle(int[] sequence, Random rnd)
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        private static void PrintHelp()
        {
            var helpText = 
            "You can do one of three things..." + Environment.NewLine +
            "Either (C)reate training set, (T) with the training set, or (R)un the ANN by drawing a capital A." + Environment.NewLine +
            "So, either C, T, or R.  Additionally, you can turn 'visuals on' or 'visuals off' depending on if " + Environment.NewLine + 
            "you want to see pretty pictures or not. You can (Q) too if you want... I don't care.";
            Console.Write(helpText);
            Console.WriteLine();

            PrintPrompt();
        }

        private static void PrintPrompt()
        {
            Console.Write("ANN: ");
        }

        private static IEnumerable<ImageSampler> ReadTrainingSet(string trainingSetDirectory)
        {
            var viewer = new ImageViewer();

            if (prettyPictures)
                Task.Run(() => { Application.Run(viewer); viewer.Show(); });
            else
                Console.WriteLine("This may take a while... Press (K) to kill.");

            var ledgerPath = Path.Combine(trainingSetDirectory, "ledger.txt");
            var count = 1;
            var updateAt = 25;
            var lines = File.ReadAllLines(ledgerPath);
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

        private static string GetExePath()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return System.IO.Path.GetDirectoryName(location);
        }
    }
}
