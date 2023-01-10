using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeParser.Commands
{
    public class G90Command : BaseCommand
    {
        public G90Command() : base("G90", GCodeCommand.G90)
        {

        }

        public G90Command(string command) : base(command, GCodeCommand.G90)
        {

        }
    }
}
