using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeParser.Commands
{
    public class M5Command: BaseCommand
    {
        public M5Command() : base("M5", GCodeCommand.M5)
        {

        }

        public M5Command(string command) : base(command, GCodeCommand.M5)
        {
        }
    }
}
