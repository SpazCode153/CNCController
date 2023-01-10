namespace GCodeParser.Commands
{
    public abstract class BaseCommand
    {
        public GCodeCommand CommandType { get; private set; }

        public string Command { get; protected set; }

        public BaseCommand(string command, GCodeCommand commandType)
        {
            Command = command;
            CommandType = commandType;
        }

        public virtual void Read(string line)
        {

        }
    }

    public enum GCodeCommand
    {
        G0,
        G1,
        G4,
        G21,
        G90,
        M3,
        M5
    }

}
