using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalReceiver
{
    public class SignalHub : Hub
    {
        public async Task SendSignalData(List<double> signalData)
        {
            await Clients.All.SendAsync("ReceiveSignalData", signalData);
        }
    }
}
