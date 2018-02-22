using AmIAnA.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmIAnA.Commands
{
    public class DetectCommand : ICommand
    {
        public bool Execute(string command, State state)
        {
            var encodedCommand = command.ToUpper();
            if (encodedCommand != "D")
            {
                return false;
            }

            var viewer = new Detect(state.NeuralNetwork);
            Task.Run(() => { viewer.Show(); Application.Run(viewer); });

            return true;
        }

        public string GetHelp()
        {
            return "(D)etect command will open a window where you can draw an image.  If the ANN detects an A, it will" + Environment.NewLine +
                   "    turn the window green.  Otherwise, you'll see red.";
        }
    }
}
