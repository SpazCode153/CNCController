using API.Machines;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class CommunicationHub: Hub
    {
        private MarlinMachineHandler machineHandler;

        public CommunicationHub(MarlinMachineHandler machineHandler)
        {
            this.machineHandler = machineHandler;
        }

        public void SendMessage(string message)
        {
            machineHandler.SendMessage(message + "\n");
        }

        public void Set(string comPort, int baudRate, int parity, int databits, int stopbits)
        {
            machineHandler.SetCommunicatonSettings(comPort, baudRate, (System.IO.Ports.Parity)parity, databits, (System.IO.Ports.StopBits)stopbits);
        }

        public void Connect()
        {
            machineHandler.Start();
        }

        public void Disconnect()
        {
            machineHandler.Stop();
        }
    }
}
