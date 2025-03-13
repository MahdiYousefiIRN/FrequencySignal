using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Services;

namespace SignalGenerator.Service
{
    public class SignalGeneratorService
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private Timer? _timer;

        public SignalGeneratorService(IHubContext<SignalHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void StartSignalGeneration(int numberOfSignals, double minFrequency, double maxFrequency, int intervalInMilliseconds)
        {
            // اگر تایمر قبلاً در حال اجرا است، ابتدا آن را متوقف کنیم
            StopSignalGeneration();

            // تایمر جدید برای تولید سیگنال‌ها
            _timer = new Timer(GenerateSignal, new { numberOfSignals, minFrequency, maxFrequency }, 0, intervalInMilliseconds);
        }

        private async void GenerateSignal(object? state)
        {
            if (state is null) return;

            var parameters = (dynamic)state;
            var random = new Random();
            var signalData = new List<double>();

            // تولید سیگنال‌ها بر اساس پارامترهای ورودی
            for (int i = 0; i < parameters.numberOfSignals; i++)
            {
                double frequency = random.NextDouble() * (parameters.maxFrequency - parameters.minFrequency) + parameters.minFrequency;
                signalData.Add(frequency);
            }

            // ارسال داده‌ها به UI از طریق SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSignalData", signalData);
        }

        public void StopSignalGeneration()
        {
            // تایمر را متوقف و آزادسازی منابع
            _timer?.Dispose();
            _timer = null; // اطمینان از این‌که تایمر دوباره مقداردهی نخواهد شد
        }
    }
}
