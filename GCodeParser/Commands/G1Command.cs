using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeParser.Commands
{
    public class G1Command : BaseCommand
    {
        public double? EPosition { get; private set; }
        public double? XPosition { get; private set; }
        public double? YPosition { get; private set; }
        public double? ZPosition { get; private set; }
        public double? FeedRate { get; private set; }
        public double? LaserPower { get; private set; }

        public G1Command() : base("G1", GCodeCommand.G1)
        {

        }

        public G1Command(string command) : base(command, GCodeCommand.G1)
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
                    case 'X':
                        {
                            XPosition = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'Y':
                        {
                            YPosition = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'Z':
                        {
                            ZPosition = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'F':
                        {
                            FeedRate = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'S':
                        {
                            LaserPower = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                }
            }
        }
    }
}
