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
        private readonly int inputCount;
        private readonly int hiddenCount;
        private readonly int outputCount;

        private readonly double[] inputs;
        private readonly double[][] inputHiddenWeights;
        private readonly double[] hiddenBiases;
        private readonly double[] hiddenOutputs;
        private readonly double[][] hiddenOutputWeights;
        private readonly double[] outputBiases;
        private readonly double[] outputs;

        private Random random;

        public NeuralNetwork(int inputNodes, int hiddenNodes, int outputNodes)
        {
            inputCount = inputNodes;
            hiddenCount = hiddenNodes;
            outputCount = outputNodes;

            inputs = new double[inputNodes];
            inputHiddenWeights = MakeMatrix(inputCount, hiddenCount, 0.0);
            hiddenBiases = new double[hiddenCount];
            hiddenOutputs = new double[hiddenCount];

            hiddenOutputWeights = MakeMatrix(hiddenCount, outputCount, 0.0);
            outputBiases = new double[outputCount];
            outputs = new double[outputCount];

            random = new Random(0);
            InitializeWeightsAndBiases();
        }

        public void SetWeights(double[] weights)
        {
            int numberOfWeights = CalculateNumberOfWeights();

            if (weights.Length != numberOfWeights)
                throw new ArgumentException(nameof(weights));

            int k = 0;

            for (var i = 0; i < inputCount; ++i)
            {
                for (var j = 0; j < hiddenCount; ++j)
                {
                    inputHiddenWeights[i][j] = weights[k++];
                }
            }

            for (var i = 0; i < hiddenCount; ++i)
            {
                hiddenBiases[i] = weights[k++];
            }

            for (var i = 0; i < hiddenCount; ++i)
            {
                for (var j = 0; j < outputCount; ++j)
                {
                    hiddenOutputWeights[i][j] = weights[k++];
                }
            }

            for (var i = 0; i < outputCount; ++i)
            {
                outputBiases[i] = weights[k++];
            }
        }

        public double[] Train(double[][] trainingData, int maxEpochs,
          double learnRate, double momentum)
        {
            double[][] hiddenOutputGradients = MakeMatrix(hiddenCount, outputCount, 0.0);
            double[] outputBiasGradients = new double[outputCount];

            double[][] inputHiddenGradients = MakeMatrix(inputCount, hiddenCount, 0.0);
            double[] hiddenBiasGradients = new double[hiddenCount];

            double[] outputSignals = new double[outputCount];
            double[] hiddenSignals = new double[hiddenCount];

            double[][] inputHiddenPreviousWeightsDelta = MakeMatrix(inputCount, hiddenCount, 0.0);
            double[] hiddenPreviousBiasesDelta = new double[hiddenCount];
            double[][] hiddenOutputPreviousWeightsDelta = MakeMatrix(hiddenCount, outputCount, 0.0);
            double[] outputPreviousBiasesDelta = new double[outputCount];

            int epoch = 0;
            double[] xValues = new double[inputCount];
            double[] tValues = new double[outputCount];
            double derivative = 0.0;
            double errorSignal = 0.0;

            int[] sequence = new int[trainingData.Length];
            for (var i = 0; i < sequence.Length; ++i)
            {
                sequence[i] = i;
            }

            int reportInterval = maxEpochs / 10;
            while (epoch < maxEpochs)
            {
                ++epoch;


                Shuffle(sequence);
                for (int ii = 0; ii < trainingData.Length; ++ii)
                {
                    var index = sequence[ii];

                    Array.Copy(trainingData[index], tValues, outputCount);
                    Array.Copy(trainingData[index], outputCount, xValues, 0, inputCount);

                    ComputeOutputs(xValues);  //  Use the NN as is to get the outputs. 

                    for (var k = 0; k < outputCount; ++k)
                    {
                        errorSignal = tValues[k] - outputs[k];
                        derivative = (1 - outputs[k]) * outputs[k];
                        outputSignals[k] = errorSignal * derivative;
                    }

                    for (var j = 0; j < hiddenCount; ++j)
                    {
                        for (var k = 0; k < outputCount; ++k)
                        {
                            hiddenOutputGradients[j][k] = outputSignals[k] * hiddenOutputs[j];
                        }
                    }

                    for (var k = 0; k < outputCount; ++k)
                    {
                        outputBiasGradients[k] = outputSignals[k] * 1.0;
                    }

                    for (var j = 0; j < hiddenCount; ++j)
                    {
                        derivative = (1 + hiddenOutputs[j]) * (1 - hiddenOutputs[j]);
                        double sum = 0.0;

                        for (var k = 0; k < outputCount; ++k)
                        {
                            sum += outputSignals[k] * hiddenOutputWeights[j][k];
                        }

                        hiddenSignals[j] = derivative * sum;
                    }

                    for (var i = 0; i < inputCount; ++i)
                    {
                        for (var j = 0; j <hiddenCount; ++j)
                        {
                            inputHiddenGradients[i][j] = hiddenSignals[j] * inputs[i];
                        }
                    }

                    for (var j = 0; j < hiddenCount; ++j)
                    {
                        hiddenBiasGradients[j] = hiddenSignals[j] * 1.0;
                    }

                    for (var i = 0; i < inputCount; ++i)
                    {
                        for (var j = 0; j < hiddenCount; ++j)
                        {
                            var delta = inputHiddenGradients[i][j] * learnRate;
                            inputHiddenWeights[i][j] += delta;
                            inputHiddenWeights[i][j] += inputHiddenPreviousWeightsDelta[i][j] * momentum;
                            inputHiddenPreviousWeightsDelta[i][j] = delta;
                        }
                    }

                    for (var j = 0; j < hiddenCount; ++j)
                    {
                        var delta = hiddenBiasGradients[j] * learnRate;
                        hiddenBiases[j] += delta;
                        hiddenBiases[j] += hiddenPreviousBiasesDelta[j] * momentum;
                        hiddenPreviousBiasesDelta[j] = delta;
                    }

                    for (int j = 0; j < hiddenCount; ++j)
                    {
                        for (var k = 0; k < outputCount; ++k)
                        {
                            var delta = hiddenOutputGradients[j][k] * learnRate;
                            hiddenOutputWeights[j][k] += delta;
                            hiddenOutputWeights[j][k] += hiddenOutputPreviousWeightsDelta[j][k] * momentum;
                            hiddenOutputPreviousWeightsDelta[j][k] = delta;
                        }
                    }

                    for (int k = 0; k < outputCount; ++k)
                    {
                        double delta = outputBiasGradients[k] * learnRate;
                        outputBiases[k] += delta;
                        outputBiases[k] += outputPreviousBiasesDelta[k] * momentum;
                        outputPreviousBiasesDelta[k] = delta;

                    }
                }
            }

            double[] bestWeights = GetWeights();

            return bestWeights;
        }

        private double[] GetWeights()
        {
            var numberOfWeights = CalculateNumberOfWeights();

            var result = new double[numberOfWeights];


            return result;
        }

        private double[] ComputeOutputs(double[] xValues)
        {
            double[] hiddenSums = new double[hiddenCount];
            double[] outputSums = new double[outputCount];

            for (var i = 0; i < xValues.Length; ++i)
            {
                inputs[i] = xValues[i];
            }

            for (var j = 0; j < hiddenCount; ++j)
            {
                for (var i = 0; i < inputCount; ++i)
                {
                    hiddenSums[j] += inputs[i] * inputHiddenWeights[i][j];
                }
            }

            for (var i = 0; i < hiddenCount; ++i)
            {
                hiddenSums[i] += hiddenBiases[i];
            }

            for (var i = 0; i < hiddenCount; ++i)
            {
                hiddenOutputs[i] = Math.Tanh(hiddenSums[i]);
            }

            for (var j = 0; j < outputCount; ++j)
            {
                for (var i = 0; i < hiddenCount; ++i)
                {
                    outputSums[j] += hiddenOutputs[i] * hiddenOutputWeights[i][j];
                }
            }

            for (var i = 0; i < outputCount; ++i)
            {
                outputSums[i] += outputBiases[i];
            }

            double[] softOut = Softmax(outputSums); 
            Array.Copy(softOut, outputs, softOut.Length);

            double[] result = new double[outputCount];
            Array.Copy(outputs, result, result.Length);
            Console.WriteLine($"{result[0]} & {result[1]}");
            return result;
        }

        private static double[] Softmax(double[] oSums)
        {
            // does all output nodes at once so scale
            // doesn't have to be re-computed each time

            double sum = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                sum += Math.Exp(oSums[i]);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i]) / sum;

            return result; // now scaled so that xi sum to 1.0
        }

        private void Shuffle(int[] sequence)
        {
            for (var i = 0; i < sequence.Length; ++i)
            {
                var r = random.Next(i, sequence.Length);
                var tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        private void InitializeWeightsAndBiases()
        {
            int numberOfWeights = CalculateNumberOfWeights();

            double[] initialWeights = new double[numberOfWeights];

            for (var i = 0; i < numberOfWeights; ++i)
            {
                initialWeights[i] = (0.001 - 0.0001) * random.NextDouble() + 0.0001;
            }

            SetWeights(initialWeights);
        }

        private int CalculateNumberOfWeights()
        {
            return (inputCount * hiddenCount) +
                (hiddenCount * outputCount) +
                hiddenCount + outputCount;
        }

        private static T [][] MakeMatrix<T>(int rows, int columns, T initialValue)
        {
            T[][] result = new T[rows][];

            for (int r = 0; r < result.Length; ++r)
            {
                result[r] = Enumerable.Repeat((T)initialValue, columns).ToArray(); ;
            }

            return result;
        }
    }
}
