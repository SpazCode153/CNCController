using API.Communication.Messages;
using System.IO.Ports;
using System.Reactive.Linq;

namespace API.Communication
{
    public class SerialCommunicationHandler: BaseCommunicationHandler<SerialMessage>
    {
        private object comPortSendLock = new object();

        private UInt16 messageSendWaitIntervalMs;

        public string COMPort { get; set; }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; }

        public int DataBits { get; set; }

        public StopBits StopBits { get; set; }

        public SerialPort comPort;

        private IObservable<byte[]> serialDataStream;

        private IObservable<SerialMessage> messageStream;

        private IDisposable messageStreamSubscription;

        public SerialCommunicationHandler(string comPortName,
            Func<byte[], Tuple<bool, int>> chunkPredicate,
            int baudRate = 9600,
            Parity parity = Parity.None,
            int databits = 8,
            StopBits stopBits = StopBits.One,
            UInt16 messageSendWaitIntervalMs = 5,
            int readBufferSize = 8192,
            int writeBufferSize = 8192)
        {
            COMPort = comPortName;
            ChunkPredicate = chunkPredicate;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = databits;
            StopBits = stopBits;
            this.messageSendWaitIntervalMs = messageSendWaitIntervalMs;

            comPort = new SerialPort();
            if (!string.IsNullOrEmpty(comPortName))
            {
                comPort.PortName = comPortName;
            }
            comPort.WriteBufferSize = 8192;
            comPort.ReadBufferSize = 8192;
            comPort.BaudRate = baudRate;
            comPort.StopBits = stopBits;
            comPort.Parity = parity;

            serialDataStream = Observable.FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>
                (
                    handler => comPort.DataReceived += handler,
                    handler => comPort.DataReceived -= handler
                )
                .Select(b =>
                {
                    int size = comPort.BytesToRead;
                    byte[] data = new byte[size];

                    comPort.Read(data, 0, size);

                    return data;
                });

            messageStream = Observable.Create<SerialMessage>(observer =>
            {
                buffer = new List<byte>();

                return serialDataStream.Subscribe(receivedByteData =>
                {
                    foreach (byte byteValue in receivedByteData)
                    {
                        buffer.Add(byteValue);

                        Tuple<bool, int> result = ChunkPredicate(buffer.ToArray());

                        if (result.Item1)
                        {
                            observer.OnNext(new SerialMessage(buffer.Take(result.Item2).ToArray()));
                        }

                        buffer.RemoveRange(0, result.Item2);
                    }
                });
            });
        }

        public override void Connect()
        {
            if (!comPort.IsOpen)
            {
                if (string.IsNullOrEmpty(COMPort))
                {
                    throw new InvalidOperationException("COM Port does not exist!");
                }
                else
                {
                    comPort.PortName = COMPort;
                    comPort.BaudRate = BaudRate;
                    comPort.Parity = Parity;
                    comPort.DataBits = DataBits;
                    comPort.StopBits = StopBits;

                    comPort.Open();

                    messageStreamSubscription?.Dispose();
                    messageStreamSubscription = null;

                    connectableMessageStream = messageStream.Publish();
                    messageStreamSubscription = MessageStream.Connect();
                }
            }
        }

        public override void Disconnect()
        {
            if (comPort.IsOpen)
            {
                try
                {
                    comPort.Close();
                }
                catch (Exception)
                { }
            }

            messageStreamSubscription?.Dispose();
            messageStreamSubscription = null;
        }

        public override bool IsConnected()
        {
            return comPort != null && comPort.IsOpen;
        }

        public override void SendMessage(SerialMessage message)
        {
            lock (comPortSendLock)
            {
                if(comPort == null || !comPort.IsOpen)
                {
                    throw new InvalidOperationException("COM port is closed, please open COM port");
                }

                if (messageSendWaitIntervalMs > 0)
                {
                    Task.Delay(messageSendWaitIntervalMs).Wait();
                }

                comPort.Write(message.MessagePayload, 0, message.MessagePayload.Length);

                if (messageSendWaitIntervalMs > 0)
                {
                    Task.Delay(messageSendWaitIntervalMs).Wait();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (comPort.IsOpen)
                    {
                        comPort.Close();
                    }

                    comPort.Dispose();
                    comPort = null;

                    messageStreamSubscription?.Dispose();
                    messageStreamSubscription = null;
                }

                disposedValue = true;
            }
        }
    }
}
