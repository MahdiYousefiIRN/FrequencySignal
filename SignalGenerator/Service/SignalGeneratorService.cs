using Microsoft.AspNetCore.SignalR;

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
            StopSignalGeneration(); // ✅ برای اطمینان از اینکه تایمر قبلی متوقف شود

            _timer = new Timer(GenerateSignal, new { numberOfSignals, minFrequency, maxFrequency }, 0, intervalInMilliseconds);
        }

        private async void GenerateSignal(object? state)
        {
            if (state is null) return;

            var parameters = (dynamic)state;
            var random = new Random();
            var signalData = new List<double>();

            for (int i = 0; i < parameters.numberOfSignals; i++)
            {
                double frequency = random.NextDouble() * (parameters.maxFrequency - parameters.minFrequency) + parameters.minFrequency;
                signalData.Add(frequency);
            }

            // ارسال داده به UI از طریق SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSignalData", signalData);
        }

        public void StopSignalGeneration()
        {
            _timer?.Dispose();
            _timer = null; // ✅ مقداردهی مجدد برای جلوگیری از مشکلات اجرای مجدد
        }
    }
}
