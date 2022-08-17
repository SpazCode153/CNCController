using API.Hubs;
using API.Machines;
using Microsoft.AspNetCore.SignalR;

namespace API.Connectors
{
    public class CommunicationConnector
    {
        public CommunicationConnector(MarlinMachineHandler machineHandler, IHubContext<CommunicationHub> hub)
        {
            machineHandler.SerialStream.Subscribe(async message =>
            {
                await hub.Clients.All.SendAsync("newSerialMessage", message);
            });
        }
    }
}
