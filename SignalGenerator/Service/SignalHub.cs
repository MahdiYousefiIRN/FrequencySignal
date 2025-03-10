using Microsoft.AspNetCore.SignalR;


namespace SignalGenerator.Service
{
    public class SignalHub : Hub
    {
        public async Task SendSignalData(List<double> signals)
        {
            await Clients.All.SendAsync("ReceiveSignalData", signals);
        }
    }
}
