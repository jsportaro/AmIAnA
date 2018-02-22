using AmIAnA.AI;
using AmIAnA.Commands;
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
        static void Main(string[] args)
        {
            var commands = new ICommand[]
            {
                new CreateTrainingCommand(),
                new DetectCommand(),
                new TrainCommand()
            };
            var state = new State();
            Console.WriteLine("Type 'H' for help.");
            do
            {
                PrintPrompt();
                var action = Console.ReadLine().ToUpper();

                var couldAnybodyExecute = commands.Any(c => c.Execute(action, state));

                if (action == "Q")
                {
                    break;
                }
                if (action == "H")
                {
                    foreach (var h in commands.Select(c => c.GetHelp()))
                    {
                        Console.WriteLine(h);
                    }
                    continue;
                }
                if (!couldAnybodyExecute)
                {
                    Console.WriteLine("Unknown command");
                }
            } while (true);
        }

        private static void PrintPrompt()
        {
            Console.Write("ANN: ");
        }
    }
}
