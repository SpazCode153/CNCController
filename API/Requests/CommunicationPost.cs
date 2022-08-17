using System.IO.Ports;

namespace API.Requests
{
    public class CommunicationPost
    {
        public string ComPort { get; set; }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; } 
        
        public int Databits { get; set; }

        public StopBits Stopbits { get; set; }
    }
}
