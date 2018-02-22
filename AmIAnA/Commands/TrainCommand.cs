using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Commands
{
    class TrainCommand : ICommand
    {
        public bool Execute(string command, State state)
        {
            var encodedCommand = command.ToUpper();
            if (encodedCommand != "T")
            {
                return false;
            }

            var path = state.GetWeightDataPath;

            if (!File.Exists(state.GetTrainingDataPath))
            {
                Console.WriteLine("Run the (C)reate command first");
                return true;
            }

            var trainingData = File.ReadAllLines(state.GetTrainingDataPath).Select(l =>
            {
                var raw = l.Split(new char[] { ',' }).Select(i => Double.Parse(i));
                return raw.ToArray();
            }).ToArray();


            double[] xValues = new double[state.NumberOfInputs];
            double[] tValues = new double[state.NumberOfOutputs];

            Console.WriteLine($"Creating a {state.NumberOfInputs}-input, {state.NumberOfHidden}-hidden, {state.NumberOfOutputs}-output neural network");
            Console.WriteLine("Using hard-coded tanh function for hidden layer activation");
            Console.WriteLine("Using hard-coded log-sigmoid function for output layer activation");


            Console.WriteLine("\nGenerating random initial weights and bias values");
            double[] initWeights = new double[state.NumberOfWeights];
            for (int i = 0; i < initWeights.Length; ++i)
                initWeights[i] = (0.1 - 0.01) * state.Random.NextDouble() + 0.01;

            Console.WriteLine("Loading neural network initial weights and biases into neural network");
            state.NeuralNetwork.SetWeights(initWeights);

            double learnRate = 0.8;  // learning rate - controls the maginitude of the increase in the change in weights.
            double momentum = 0.2; // momentum - to discourage oscillation.
            Console.WriteLine($"Setting learning rate = {learnRate.ToString("F2")} and momentum = {momentum.ToString("F2")}");

            var maxEpochs = 1000;
            var errorThresh = 0.00001;
            Console.WriteLine($"Setting max epochs = {maxEpochs} and error threshold = {errorThresh.ToString("F6")}");

            Console.WriteLine("\nBeginning training using back-propagation\n");
            double[] weights = state.NeuralNetwork.Train(trainingData, maxEpochs, learnRate, momentum);

            File.WriteAllText(path, string.Join(",", weights));

            return true;
        }

        public string GetHelp()
        {
            return "(T)rains the ANN with the training vectors (C)reated by the C command";
        }
    }
}
