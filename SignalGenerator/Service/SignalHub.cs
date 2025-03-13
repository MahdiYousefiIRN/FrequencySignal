using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Models;
using System.Threading.Tasks;

namespace SignalGenerator.Services
{
    public class SignalHub : Hub
    {
        // ارسال داده‌های سیگنال به همه کلاینت‌ها
        public async Task SendSignalData(List<SignalData> signals)
        {
            await Clients.All.SendAsync("ReceiveSignalData", signals);
        }

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

        // ارسال وضعیت سیگنال
        public async Task SendSignalStatus(SignalData signal)
        {
            await Clients.All.SendAsync("UpdateSignalStatus", signal);
        }
    }
}
