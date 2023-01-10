using API.Machines;
using API.Protocols.Marlin;
using GCodeParser;
using GCodeParser.Commands;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;
using G0Command = GCodeParser.Commands.G0Command;

namespace ConsoleCommand
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##########################################################");
            Console.WriteLine($"CNC Controller Commandline V{Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("##########################################################");
            Console.WriteLine("Type 'Help' for a list of commands");

            bool isRunning = true;

            string command = "";

            Type thisType = typeof(Commands);
            Commands commands = new Commands();

            while (isRunning)
            {
                Console.Write("Command: ");

                command = Console.ReadLine();

                if (command != "Exit")
                {
                    if (command == "Clear")
                    {
                        Console.Clear();
                        Console.WriteLine("##########################################################");
                        Console.WriteLine($"CNC Controller Commandline V{Assembly.GetExecutingAssembly().GetName().Version}");
                        Console.WriteLine("##########################################################");
                        Console.WriteLine("Type 'Help' for a list of commands");
                    }
                    else
                    {
                        MethodInfo theMethod = thisType.GetMethod(command);

                        if (theMethod != null)
                        {
                            theMethod.Invoke(commands, null);
                        }
                        else
                        {
                            Console.WriteLine("Unknown Command, please execute 'Help' for a list of commands");
                        }
                    }
                }
                else
                {
                    Console.Write($"Shutting down application....");
                    for (int i = 6; i-- > 1;)
                    {
                        Console.CursorLeft = Console.CursorLeft - 1;
                        Console.Write($"{i}");
                        Thread.Sleep(1000);
                    }

                    isRunning = false;
                }
            }
        }
    }

    public class Commands
    {
        private MarlinMachineHandler machineHandler = new MarlinMachineHandler();

        Dictionary<uint, BaseCommand> gcodeFileCommands;

        IEnumerable<Position> heightPositions;

        List<PositionSquare> squares;

        public void Help()
        {
            Console.WriteLine("Here are a list of commands that can be used:");
            Console.WriteLine("{0, -30}{1, 10}",
                "Help",
                "Displays a list of commands and there descriptions");
            Console.WriteLine("{0, -30}{1, 10}",
                "SetComms",
                "Sets the communication settings");
            Console.WriteLine("{0, -30}{1, 10}",
                "Connect",
                "Connects to serial port");
            Console.WriteLine("{0, -30}{1, 10}",
                "Disconnect",
                "Disconnects from serial port");
            Console.WriteLine("{0, -30}{1, 10}",
                "Home",
                "Perform a G28 homing command");
            Console.WriteLine("{0, -30}{1, 10}",
                "Move",
                "Perform a G0 move command");
            Console.WriteLine("{0, -30}{1, 10}",
                "Probe",
                "Perform a G30 probing command");
            Console.WriteLine("{0, -30}{1, 10}",
                "Exit",
                "Shutdown the application");
            Console.WriteLine("");
        }

        public void SetComms()
        {
            Console.Write("What is the COM port? eg. COM11: ");
            string comPort = Console.ReadLine();
            Console.Write("What is the baud rate? eg. 115200: ");
            string baudRate = Console.ReadLine();
            Console.Write("What is the data bits? eg. 8: ");
            string databits = Console.ReadLine();
            Console.Write("What is the stop bits? eg. One: ");
            string stopbits = Console.ReadLine();
            StopBits portStopbits = StopBits.One;
            switch (stopbits)
            {
                case "OnePointFive":
                    {
                        portStopbits = StopBits.OnePointFive;

                        break;
                    }
                case "None":
                    {
                        portStopbits = StopBits.None;

                        break;
                    }
            }

            Console.Write("What is the parity? eg. None: ");
            string parity = Console.ReadLine();
            Parity portParity = Parity.None;
            switch(parity)
            {
                case "Even":
                    {
                        portParity = Parity.Even;

                        break;
                    }
                case "Odd":
                    {
                        portParity = Parity.Odd;

                        break;
                    }
                case "Mark":
                    {
                        portParity = Parity.Mark;

                        break;
                    }
            }

            machineHandler.SetCommunicatonSettings(comPort, int.Parse(baudRate), portParity, int.Parse(databits), portStopbits);
            Console.WriteLine("");
        }

        public void Connect()
        {
            Console.WriteLine("Connecting....");
            try
            {
                machineHandler.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Connection Failed! { ex.Message }");
            }
        }

        public void Discconnect()
        {
            Console.WriteLine("Disconnecting....");
            try
            {
                machineHandler.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disonnection Failed! {ex.Message}");
            }
        }

        public void Home()
        {
            Console.WriteLine("Homing...");
            try
            {
                machineHandler.Home(true, true, true).Wait();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Homing Failed! { ex.Message }");
            }
        }

        public void Move()
        {
            Console.Write("X position: ");
            string xPos = Console.ReadLine();
            Console.Write("Y position: ");
            string yPos = Console.ReadLine();
            Console.Write("Z position: ");
            string zPos = Console.ReadLine();

            Console.WriteLine("Move...");
            try
            {
                machineHandler.Move(double.Parse(xPos), double.Parse(yPos), double.Parse(zPos)).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Move Failed! {ex.Message}");
            }
        }

        public void Probe()
        {
            Console.Write("X position: ");
            string xPos = Console.ReadLine();
            Console.Write("Y position: ");
            string yPos = Console.ReadLine();

            Console.WriteLine("Probing...");
            try
            {
                Position position = machineHandler.Probe(double.Parse(xPos), double.Parse(yPos)).Result;
                Console.WriteLine($"Position: X{position.X} Y{position.Y}   Z Height:{position.Z}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Probe Failed! {ex.Message}");
            }
        }

        public void ProbeGrid()
        {
            Console.Write("Start X position: ");
            string startXPos = Console.ReadLine();
            Console.Write("End X position: ");
            string endXPos = Console.ReadLine();
            Console.Write("Start Y position: ");
            string startYPos = Console.ReadLine();
            Console.Write("End Y position: ");
            string endYPos = Console.ReadLine();
            Console.Write("Gap: ");
            string gap = Console.ReadLine();


            double startX = double.Parse(startXPos);
            double endX = double.Parse(endXPos);
            double startY = double.Parse(startYPos);
            double endY = double.Parse(endYPos);
            double gapSize = double.Parse(gap);

            try
            {
                heightPositions = probeGrid(startX, endX, startY, endY, gapSize);

                squares = new List<PositionSquare>();

                foreach (Position position in heightPositions)
                {
                    Position bottomLeft = position;

                    Position? bottomRight = heightPositions.SingleOrDefault(i => i.Y == position.Y && i.X == (position.X + gapSize));
                    
                    if(bottomRight == null)
                    {
                        continue;
                    }
                    
                    Position? topLeft = heightPositions.SingleOrDefault(i => i.X == position.X && i.Y == (position.Y + gapSize));

                    if (topLeft == null)
                    {
                        continue;
                    }

                    Position? topRight = heightPositions.SingleOrDefault(i => i.X == (position.X + gapSize) && i.Y == (position.Y + gapSize));

                    if (topRight == null)
                    {
                        continue;
                    }

                    squares.Add(new PositionSquare(topLeft, bottomLeft, topRight, bottomRight, 0.01));
                }


                foreach (Position position in heightPositions)
                {
                    Console.WriteLine($"Position: X{position.X} Y{position.Y}   Z Height:{position.Z}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Probe Failed! {ex.Message}");
            }
        }

        private IEnumerable<Position> probeGrid(double startX, double endX, double startY, double endY, double gapSize)
        {
            machineHandler.Move(startX, startY, 4.0).Wait();
            int counter = 1;

            Console.WriteLine("Probing...");

            List<Position> positions = new List<Position>();
            double totalProbePoints = (((endX + gapSize) - startX) / gapSize) * (((endY + gapSize) - startY) / gapSize);
            int pointsComplete = 0;
            int perPos = Console.CursorLeft;
            Console.Write($"Points: 0/{totalProbePoints}    0% Complete...");

            TimeSpan totalEstimatedTime = TimeSpan.Zero;
            TimeSpan lastPointTimeElapsed = TimeSpan.Zero;

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (double y = startY; y < endY + gapSize; y += gapSize)
            {
                for (double x = startX; x < endX + gapSize; x += gapSize)
                {
                    if (counter > 1)
                    {
                        machineHandler.Move(x, y, 4.0).Wait();
                    }
                    if(counter == 2)
                    {
                        totalEstimatedTime = TimeSpan.FromTicks((long)(lastPointTimeElapsed.Ticks * totalProbePoints));
                    }

                    positions.Add(machineHandler.Probe(x, y).Result);
                    machineHandler.Move(x, y, 4.0).Wait();
                    pointsComplete++;
                    Console.CursorLeft = perPos;
                    Console.Write("                                                                                                                       ");
                    Console.CursorLeft = perPos;
                    lastPointTimeElapsed = stopwatch.Elapsed;
                    Console.Write($"{lastPointTimeElapsed}/{totalEstimatedTime}  Points: {pointsComplete}/{totalProbePoints}    {((pointsComplete / totalProbePoints) * 100)}% Complete...");
                    counter++;
                }
            }
            stopwatch.Stop();

            return positions;
        }

        public void LoadFile()
        {
            Console.Write("What is the file directory?: ");
            string fileDirectory = Console.ReadLine();

            if(File.Exists(fileDirectory))
            {
                gcodeFileCommands = Parser.ReadFile(fileDirectory);
            }
            else
            {
                Console.WriteLine("File does not exist!");
            }
        }

        public void ApplyHeightMap()
        {
            double? currentX = null;
            double? currentY = null;
            double? currentZ = null;

            for(uint i = 1; i < gcodeFileCommands.Count + 1; i++)
            {
                switch(gcodeFileCommands[i].CommandType)
                {
                    case GCodeCommand.G0:
                        {
                            G0Command g0Command = (G0Command)gcodeFileCommands[i];

                            if (g0Command.XPosition != null)
                            {
                                currentX = g0Command.XPosition;
                            }
                            if (g0Command.YPosition != null)
                            {
                                currentY = g0Command.YPosition;
                            }
                            if (g0Command.ZPosition != null)
                            {
                                currentZ = g0Command.ZPosition;

                                if(currentX != null && currentY != null)
                                {
                                    PositionSquare? positionSquare = squares.SingleOrDefault(x => x.IsPositionInSquare((double)currentX, (double)currentY));

                                    if(positionSquare != null)
                                    {
                                        positionSquare.ApplyZHeight(new Position()
                                        {
                                            X = (double)currentX,
                                            Y = (double)currentY,
                                            Z = (double)currentZ
                                        });
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to apply height map, position is not in grid.");
                                        return;
                                    }
                                }
                            }

                            break;
                        }
                    case GCodeCommand.G1:
                        {
                            G1Command g1Command = (G1Command)gcodeFileCommands[i];

                            if (g1Command.XPosition != null)
                            {
                                currentX = g1Command.XPosition;
                            }
                            if (g1Command.YPosition != null)
                            {
                                currentY = g1Command.YPosition;
                            }
                            if (g1Command.ZPosition != null)
                            {
                                currentZ = g1Command.ZPosition;

                                if (currentX != null && currentY != null)
                                {
                                    PositionSquare? positionSquare = squares.SingleOrDefault(x => x.IsPositionInSquare((double)currentX, (double)currentY));

                                    if (positionSquare != null)
                                    {
                                        positionSquare.ApplyZHeight(new Position()
                                        {
                                            X = (double)currentX,
                                            Y = (double)currentY,
                                            Z = (double)currentZ
                                        });
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to apply height map, position is not in grid.");
                                        return;
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        public void ProbeGcodeFileShape()
        {
            if (gcodeFileCommands != null)
            {
                double? minXPos = null;
                double? minYPos = null;
                double? maxXPos = null;
                double? maxYPos = null;
                double? currentZPos = 0;

                for (uint i = 1; i < (gcodeFileCommands.Count + 1); i++)
                {
                    BaseCommand command = gcodeFileCommands[i];

                    if (command.CommandType == GCodeCommand.G0)
                    {
                        G0Command g0Command = (G0Command)command;

                        if (currentZPos < 0)
                        {
                            if (g0Command.XPosition != null)
                            {
                                if (minXPos != null)
                                {
                                    if (minXPos > g0Command.XPosition)
                                    {
                                        minXPos = g0Command.XPosition;
                                    }
                                }
                                else
                                {
                                    minXPos = g0Command.XPosition;
                                }

                                if (maxXPos != null)
                                {
                                    if (maxXPos < g0Command.XPosition)
                                    {
                                        maxXPos = g0Command.XPosition;
                                    }
                                }
                                else
                                {
                                    maxXPos = g0Command.XPosition;
                                }
                            }
                            if (g0Command.YPosition != null)
                            {
                                if (minYPos != null)
                                {
                                    if (minYPos > g0Command.YPosition)
                                    {
                                        minYPos = g0Command.YPosition;
                                    }
                                }
                                else
                                {
                                    minYPos = g0Command.YPosition;
                                }

                                if (maxYPos != null)
                                {
                                    if (maxYPos < g0Command.YPosition)
                                    {
                                        maxYPos = g0Command.YPosition;
                                    }
                                }
                                else
                                {
                                    maxYPos = g0Command.YPosition;
                                }
                            }
                        }
                        if (g0Command.ZPosition != null)
                        {
                            currentZPos = g0Command.ZPosition;
                        }
                    }
                    if (command.CommandType == GCodeCommand.G1)
                    {
                        G1Command g1Command = (G1Command)command;

                        if (currentZPos < 0)
                        {
                            if (g1Command.XPosition != null)
                            {
                                if (minXPos != null)
                                {
                                    if (minXPos > g1Command.XPosition)
                                    {
                                        minXPos = g1Command.XPosition;
                                    }
                                }
                                else
                                {
                                    minXPos = g1Command.XPosition;
                                }

                                if (maxXPos != null)
                                {
                                    if (maxXPos < g1Command.XPosition)
                                    {
                                        maxXPos = g1Command.XPosition;
                                    }
                                }
                                else
                                {
                                    maxXPos = g1Command.XPosition;
                                }
                            }
                            if (g1Command.YPosition != null)
                            {
                                if (minYPos != null)
                                {
                                    if (minYPos > g1Command.YPosition)
                                    {
                                        minYPos = g1Command.YPosition;
                                    }
                                }
                                else
                                {
                                    minYPos = g1Command.YPosition;
                                }

                                if (maxYPos != null)
                                {
                                    if (maxYPos < g1Command.YPosition)
                                    {
                                        maxYPos = g1Command.YPosition;
                                    }
                                }
                                else
                                {
                                    maxYPos = g1Command.YPosition;
                                }
                            }
                        }
                        if (g1Command.ZPosition != null)
                        {
                            currentZPos = g1Command.ZPosition;
                        }
                    }
                }

                Console.WriteLine($"Min X:{Math.Round((double)minXPos, MidpointRounding.ToNegativeInfinity)}");
                Console.WriteLine($"Max X:{Math.Round((double)maxXPos, MidpointRounding.ToPositiveInfinity)}");
                Console.WriteLine($"Min Y:{Math.Round((double)minYPos, MidpointRounding.ToNegativeInfinity)}");
                Console.WriteLine($"Max Y:{Math.Round((double)maxYPos, MidpointRounding.ToPositiveInfinity)}");

                if (minXPos != null && maxXPos != null && minYPos != null && maxYPos != null)
                {
                    heightPositions = probeGrid(Math.Round((double)minXPos,MidpointRounding.ToNegativeInfinity), 
                        Math.Round((double)maxXPos, MidpointRounding.ToPositiveInfinity),
                        Math.Round((double)minYPos, MidpointRounding.ToNegativeInfinity),
                        Math.Round((double)maxYPos, MidpointRounding.ToPositiveInfinity), 
                        1.0);

                    squares = new List<PositionSquare>();

                    foreach (Position position in heightPositions)
                    {
                        Position bottomLeft = position;

                        Position? bottomRight = heightPositions.SingleOrDefault(i => i.Y == position.Y && i.X == (position.X + 1));

                        if (bottomRight == null)
                        {
                            continue;
                        }

                        Position? topLeft = heightPositions.SingleOrDefault(i => i.X == position.X && i.Y == (position.Y + 1));

                        if (topLeft == null)
                        {
                            continue;
                        }

                        Position? topRight = heightPositions.SingleOrDefault(i => i.X == (position.X + 1) && i.Y == (position.Y + 1));

                        if (topRight == null)
                        {
                            continue;
                        }

                        squares.Add(new PositionSquare(topLeft, bottomLeft, topRight, bottomRight, 0.01));
                    }
                }
                else
                {
                    Console.WriteLine("Cant probe area, coordinates are invalid!");
                }
            }
            else
            {
                Console.WriteLine("Please load a GCode file");
            }
        }

        public void SaveHeightMapToFile()
        {
            if(heightPositions != null)
            {
                Console.Write("What is the file name?: ");
                string fileName = Console.ReadLine();

                using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName),false))
                {
                    foreach(Position pos in heightPositions)
                    {
                        sw.WriteLine($"{ pos.X },{ pos.Y },{ pos.Z }");
                    }
                }
            }
        }

        public void LoadHeightMapFromFile()
        {
            Console.Write("What is the file name?: ");
            string fileName = Console.ReadLine();

            List<Position> hPositions = new List<Position>();

            using (StreamReader sr = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName)))
            {
                string? line = sr.ReadLine();

                while (line != null)
                {
                    string[] coordinates = line.Split(',');

                    hPositions.Add(new Position()
                    {
                        X = double.Parse(coordinates[0]),
                        Y = double.Parse(coordinates[1]),
                        Z = double.Parse(coordinates[2])
                    });

                    line = sr.ReadLine();
                }
            }

            heightPositions = hPositions.ToArray();
        }

        public void Exit()
        {

        }
    }
}