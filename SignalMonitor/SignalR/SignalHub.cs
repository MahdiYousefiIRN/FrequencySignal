using Microsoft.AspNetCore.SignalR;
using SignalMonitor.Models;

namespace SignalMonitor.SignalR
{
    public class SignalHub : Hub
    {
       
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendSignalData(List<double> signalData)
        {
            await Clients.All.SendAsync("ReceiveSignalData", signalData);
        }
    }
}
