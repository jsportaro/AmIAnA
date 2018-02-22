using AmIAnA.AI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Commands
{
    public class State
    {
        public BackPropNeuralNet NeuralNetwork { get; private set; } 
        public Random Random { get; } = new Random(1);
        public int NumberOfInputs { get; } = 25;
        public int NumberOfHidden { get; } = 18;
        public int NumberOfOutputs { get; } = 2;

        public State()
        {
            NeuralNetwork = new BackPropNeuralNet(NumberOfInputs, NumberOfHidden, NumberOfOutputs);
        }

        public int NumberOfWeights
        {
            get
            {
                return (NumberOfInputs * NumberOfHidden) + (NumberOfHidden * NumberOfOutputs) + (NumberOfHidden + NumberOfOutputs);
            }
        }

        public string GetTrainingLedgerPath
        {
            get

            {
                return Path.Combine(GetExePath, ConfigurationManager.AppSettings["trainingLedger"]);
            }
        }

        public string GetTrainingDataPath
        {
            get
            {
                return Path.Combine(GetExePath, ConfigurationManager.AppSettings["trainingData"]);
            }
        }

        public string GetWeightDataPath
        {
            get
            {
                return Path.Combine(GetExePath, ConfigurationManager.AppSettings["weightData"]);
            }
        }

        public string GetExePath
        {
            get
            {
                var location = System.Reflection.Assembly.GetExecutingAssembly().Location;

                return System.IO.Path.GetDirectoryName(location);
            }
        }
    }
}
