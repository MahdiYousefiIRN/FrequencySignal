using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Models;
using System.Threading.Tasks;

namespace SignalGenerator.Services
{
    public class SignalHub : Hub
    {
       

        // ارسال وضعیت ارسال بسته‌ها به همه کلاینت‌ها
        public async Task SendPacketCount(int sentPackets)
        {
            await Clients.All.SendAsync("UpdateSentPackets", sentPackets);
        }

        // ارسال زمان باقی‌مانده به کلاینت‌ها
        public async Task SendTimeRemaining(double timeRemaining)
        {
            await Clients.All.SendAsync("UpdateTimeRemaining", timeRemaining);
        }

       
    }
}
