using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalGenerator
{
    public class SignalHub : Hub
    {
        // این متد برای ارسال داده‌ها به کلاینت‌ها از سمت سرور
        public async Task SendSignalDataToClients(List<double> signalData)
        {
            // ارسال داده سیگنال به تمام کلاینت‌های متصل
            await Clients.All.SendAsync("ReceiveSignalData", signalData);
        }

        // متد برای اتصال جدید (می‌توانید از این متد برای مدیریت اتصال‌ها استفاده کنید)
        public override Task OnConnectedAsync()
        {
            // می‌توانید منطق خاصی برای هر اتصال جدید اضافه کنید
            Console.WriteLine("A new client has connected.");
            return base.OnConnectedAsync();
        }

        // متد برای قطع اتصال (برای مدیریت قطع اتصال‌ها)
        public override Task OnDisconnectedAsync(Exception exception)
        {
            // منطق خاصی برای قطع اتصال می‌توانید اضافه کنید
            Console.WriteLine("A client has disconnected.");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
