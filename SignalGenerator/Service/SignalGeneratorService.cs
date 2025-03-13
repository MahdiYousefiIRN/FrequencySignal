using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Models;
using SignalGenerator.Services;

namespace SignalGenerator.Service
{
    public class SignalGeneratorService
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private Timer? _timer;
        private SignalConfigGeneration? _config; // ذخیره تنظیمات برای بروزرسانی مقدار Interval

        public SignalGeneratorService(IHubContext<SignalHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void StartSignalGeneration(SignalConfigGeneration signalConfigGeneration)
        {
            StopSignalGeneration(); // اطمینان از توقف تایمر قبلی

            _config = signalConfigGeneration; // ذخیره تنظیمات
            _timer = new Timer(GenerateSignal, null, 0, _config.IntervalMs);
        }

        private async void GenerateSignal(object? state)
        {
            if (_config is null) return;

            var random = new Random();
            var signalData = new List<double>();

            for (int i = 0; i < _config.SignalCount; i++)
            {
                double frequency = random.NextDouble() * (_config.MaxFrequency - _config.MinFrequency) + _config.MinFrequency;
                signalData.Add(frequency);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveSignalData", signalData);

            // تغییر مقدار Interval در حال اجرا بدون نیاز به ایجاد تایمر جدید
            _timer?.Change(_config.IntervalMs, Timeout.Infinite);
        }

        public void StopSignalGeneration()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public void UpdateInterval(int newInterval)
        {
            if (_config is not null && _timer is not null)
            {
                _config.IntervalMs = newInterval;
                _timer.Change(0, newInterval); // تنظیم مقدار جدید برای تایمر
            }
        }
    }
}
