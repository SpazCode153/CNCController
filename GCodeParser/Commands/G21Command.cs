using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeParser.Commands
{
    public class G21Command : BaseCommand
    {
        public G21Command() : base("G21", GCodeCommand.G21)
        {

        }

        public G21Command(string command) : base(command, GCodeCommand.G21)
        {

        }
    }
}
