using API.Machines;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;

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

            machineHandler.Move(startX, startY, 4.0).Wait();
            int counter = 1;

            Console.WriteLine("Probing...");
            try
            {
                List<Position> positions = new List<Position>();
                double totalProbePoints = ((endX - startX) / gapSize) * ((endY - startY) / gapSize);
                int pointsComplete = 0;
                int perPos = Console.CursorLeft;
                Console.Write($"Points: 0/{totalProbePoints}    0% Complete...");

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                for (double y = startY; y < endY; y = y + gapSize)
                {
                    for (double x = startX; x < endX; x = x + gapSize)
                    {
                        if(counter > 1)
                        {
                            machineHandler.Move(x, y, 4.0).Wait();
                        }

                        positions.Add(machineHandler.Probe(x, y).Result);
                        machineHandler.Move(x, y, 4.0).Wait();
                        pointsComplete++;
                        Console.CursorLeft = perPos;
                        Console.Write("                                                                                                                       ");
                        Console.CursorLeft = perPos;
                        Console.Write($"{ stopwatch.Elapsed }  Points: {pointsComplete}/{totalProbePoints}    { ((pointsComplete/totalProbePoints) * 100) }% Complete...");
                        counter++;
                    }
                }
                stopwatch.Stop();

                foreach (Position position in positions)
                {
                    Console.WriteLine($"Position: X{position.X} Y{position.Y}   Z Height:{position.Z}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Probe Failed! {ex.Message}");
            }
        }

        public void LoadFile()
        {
            Console.Write("What is the file directory?: ");
            string fileDirectory = Console.ReadLine();

            
        }

        public void Exit()
        {

        }
    }
}