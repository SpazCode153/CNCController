using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeParser.Commands
{
    public class M3Command : BaseCommand
    {
        public bool IMode { get; private set; }

        public double OPower { get; private set; }

        public double SPower { get; private set; }

        public M3Command() : base("M3", GCodeCommand.M3)
        {

        }

        public M3Command(string command) : base(command, GCodeCommand.M3)
        {
            Read(command);
        }

        public override void Read(string line)
        {
            string[] cmdValues = line.Split(' ');

            for (byte i = 1; i < cmdValues.Length; i++)
            {
                switch (cmdValues[i].ToUpper()[0])
                {
                    case 'I':
                        {
                            IMode = true;

                            break;
                        }
                    case 'O':
                        {
                            OPower = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'S':
                        {
                            SPower = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                }
            }
        }
    }
}
