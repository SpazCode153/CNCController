namespace GCodeParser.Commands
{
    public class G4Command : BaseCommand
    {
        public double? PMillisecond { get; private set; }
        public double? SSecond { get; private set; }

        public G4Command() : base("G4", GCodeCommand.G4)
        {

        }

        public G4Command(string command) : base(command, GCodeCommand.G4)
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
                    case 'P':
                        {
                            PMillisecond = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                    case 'S':
                        {
                            SSecond = double.Parse(cmdValues[i].Substring(1));

                            break;
                        }
                }
            }
        }
    }
}
