using Microsoft.AspNetCore.SignalR;
using SignalMonitor.Models;

namespace SignalMonitor.SignalR
{
    public class PacketDataHub : Hub
    {
        // متدی برای ارسال داده‌ها به همه کلاینت‌ها
        public async Task SendPacketDataAsync(PacketData packetData)
        {
            await Clients.All.SendAsync("ReceivePacketData", packetData);
        }
    }
}
