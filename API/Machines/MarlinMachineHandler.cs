using API.Communication;
using API.Communication.Messages;
using API.Protocols;
using API.Protocols.Marlin;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.IO.Ports;

namespace API.Machines
{
    public class MarlinMachineHandler
    {
        public IObservable<string> SerialStream;

        protected Action<string> newSerialMessage = null;



        public SerialCommunicationHandler communciationHandler;

        public MarlinMachineHandler()
        {
            SerialStream = Observable.FromEvent<string>(handler => newSerialMessage += handler, handler => newSerialMessage -= handler);
        }

        public void SetCommunicatonSettings(string comPort, int baudRate, Parity parity, int databits, StopBits stopbits)
        {
            communciationHandler = new SerialCommunicationHandler(comPort, 
                detectCommunication,
                baudRate,
                parity,
                databits,
                stopbits);
        }

        private Tuple<bool, int> detectCommunication(byte[] buffer)
        {
            //Detect if something is wrong
            if(buffer.Length > 500)
            {
                throw new InvalidDataException("Too much data coming from the machine, something is wrong");
            }

            for(int x = 0; x< buffer.Length; x++)
            {
                ////Detect if something is wrong
                //if (buffer[x] > 0x7E)
                //{
                //    throw new InvalidDataException("Data received from machine is not valid text");
                //}

                //Detect line feed
                if (buffer[x] == 0x0A)
                {
                    return new Tuple<bool, int>(true, x + 1);
                }
            }

            return new Tuple<bool, int>(false, 0);
        }

        public void Start()
        {
            communciationHandler.Connect();
            communciationHandler.MessageStream.Subscribe(processReceivedData);
        }

        private void processReceivedData(SerialMessage serialMessage)
        {
            newSerialMessage?.Invoke(serialMessage.Message);
        }





        public void SendMessage(string message)
        {
            communciationHandler.SendMessage(new SerialMessage(message));
        }

        private IConnectableObservable<TResponse> singleResponseStream<TResponse>(BaseGcodeCommand<TResponse> command)
            where TResponse : BaseGCodeCommandResult
        {
            return Observable.Create<TResponse>(observer =>
            {
                return communciationHandler.MessageStream.Subscribe(message =>
                {
                    try
                    {
                        TResponse result = command.ValidateResponse(message.Message);

                        observer.OnNext(result);

                        if(result.Success || result.Error)
                        {
                            observer.OnCompleted();
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);

                        observer.OnCompleted();
                    }
                });
            }).Replay();
        }

        public async Task Home(bool x, bool y, bool z)
        {
            G28Command command = new G28Command(x, y, z);

            IConnectableObservable<G28CommandResult> stream = singleResponseStream(command);

            using (stream.Connect())
            {
                communciationHandler.SendMessage(new SerialMessage(command.Command + "\n"));

                try
                {
                    G28CommandResult result = await stream.LastOrDefaultAsync();

                    if (result.Success)
                    {
                        return;
                    }

                    throw new Exception("Machine failed to perform G28 command");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task Move(double? x, double? y, double? z)
        {
            await Task.Delay(500);

            G0Command command = new G0Command(x, y, z);

            IConnectableObservable<G0CommandResult> stream = singleResponseStream(command);

            using (stream.Connect())
            {
                communciationHandler.SendMessage(new SerialMessage(command.Command + "\n"));

                try
                {
                    G0CommandResult result = await stream.LastOrDefaultAsync();

                    if(result.Success)
                    {
                        return;
                    }

                    throw new Exception("Machine failed to perform G0 command");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<Position> Probe(double? x, double? y)
        {
            await Task.Delay(500);

            G30Command command = new G30Command(x, y);

            IConnectableObservable<G30CommandResult> stream = singleResponseStream(command);

            using (stream.Connect())
            {
                communciationHandler.SendMessage(new SerialMessage(command.Command + "\n"));

                try
                {
                    G30CommandResult result = await stream.LastOrDefaultAsync();

                    if(result.Success)
                    {
                        return new Position()
                        {
                            X = result.Position[0],
                            Y = result.Position[1],
                            Z = result.Position[2]
                        };
                    }

                    throw new Exception("Machine failed to perform G30 command");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }








        public void Stop()
        {
            communciationHandler.Disconnect();
        }
    }

    public class Position
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }
    }
}
