using GCodeParser.Commands;

namespace GCodeParser
{
    public class Parser
    {
        public static Dictionary<uint, BaseCommand> ReadFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                Dictionary<uint, BaseCommand> result = new Dictionary<uint, BaseCommand>();
                uint lineCounter = 1;
                string? line = reader.ReadLine();
                while(line != null)
                {
                    string[] values = line.Split(' ');
                    result.Add(lineCounter, Commands.Commands.GetCommand(values[0], line));

                    lineCounter++;
                    line = reader.ReadLine();
                }

                return result;
            }
        }
    }
}