using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmIAnA.Commands
{
    internal interface ICommand
    {
        bool Execute(string command, State state);

        string GetHelp();
    }
}
