using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Modbus;
using SignalGenerator.Models;
using SignalGenerator.Services;
using System.Text;
using System.Text.Json;

public class SignalGeneratorService
{
    private readonly IHubContext<SignalHub> _hubContext;
    private readonly ModbusClientManager _modbusClientManager;
    private readonly ILogger<SignalGeneratorService> _logger;
    private readonly HttpClient _httpClient;

    private SignalConfigGeneration? _config;
    private CancellationTokenSource? _cancellationTokenSource;
    private Timer? _signalGenerationTimer;

    private const string RequestUriAPI = "http://localhost:5002/api/packetdata";

    // کال‌بک برای به روز رسانی سیگنال‌ها
    private Action<List<double>> _updateSignalDataCallback;

    public SignalGeneratorService(
        IHubContext<SignalHub> hubContext,
        ModbusClientManager modbusClientManager,
        ILogger<SignalGeneratorService> logger,
        HttpClient httpClient)
    {
        _hubContext = hubContext;
        _modbusClientManager = modbusClientManager;
        _logger = logger;
        _httpClient = httpClient;
    }

    // این متد برای اتصال کال‌بک UI به سرویس است
    public void SetUpdateSignalDataCallback(Action<List<double>> updateSignalDataCallback)
    {
        _updateSignalDataCallback = updateSignalDataCallback;
    }

    public async Task<string> StartSignalGeneration(SignalConfigGeneration signalConfigGeneration, CancellationToken cancellationToken)
    {
        StopSignalGeneration();

        _config = signalConfigGeneration ?? throw new ArgumentNullException(nameof(signalConfigGeneration));

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _signalGenerationTimer = new Timer(async _ =>
        {
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                await GenerateSignals(_cancellationTokenSource.Token);
            }
        }, null, 0, _config.IntervalMs);

        return "Signal generation started successfully.";
    }

    public void StopSignalGeneration()
    {
        _signalGenerationTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _signalGenerationTimer?.Dispose();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _logger.LogInformation("Signal generation stopped.");
    }

    private async Task GenerateSignals(CancellationToken token)
    {
        if (_config == null || token.IsCancellationRequested) return;
        try
        {
            _logger.LogInformation("Generating signals...");
            var random = new Random();
            var signalData = Enumerable.Range(0, _config.SignalCount)
                                       .Select(_ => random.NextDouble() * (_config.MaxFrequency - _config.MinFrequency) + _config.MinFrequency)
                                       .ToList();

            await SendSignalToSignalR(signalData);
            await SendSignalToModbus(signalData);
            await SendSignalToHttp(signalData);

            // ارسال داده‌ها به کال‌بک UI
            _updateSignalDataCallback?.Invoke(signalData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in signal generation: {ex.Message}");
        }
    }

    private async Task SendSignalToSignalR(List<double> signalData)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveSignalData", signalData);
            _logger.LogInformation("Signal sent to SignalR clients.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending signal to SignalR: {ex.Message}");
        }
    }

    private async Task SendSignalToModbus(List<double> signalData)
    {
        try
        {
            if (!_modbusClientManager.IsConnected) _modbusClientManager.Connect();
            _modbusClientManager.WriteModbusData(0, signalData.Select(d => (int)(d * 1000)).ToArray());
            _logger.LogInformation("Signal sent to Modbus.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending signal to Modbus: {ex.Message}");
        }
    }

    private async Task SendSignalToHttp(List<double> signalData)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(new { SignalData = signalData, Timestamp = DateTime.Now }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(RequestUriAPI, content);
            _logger.LogInformation(response.IsSuccessStatusCode ? "Signal sent to HTTP API successfully." : $"Failed to send signal to HTTP API. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending signal to HTTP API: {ex.Message}");
        }
    }

    // متدهای جدید برای به روز رسانی تنظیمات

    // به روز رسانی تعداد سیگنال‌ها
    public void UpdateSignalCount(int signalCount)
    {
        if (_config != null)
        {
            _config.SignalCount = signalCount;
        }
    }

    // به روز رسانی بازه فرکانس
    public void UpdateFrequencyRange(double minFrequency, double maxFrequency)
    {
        if (_config != null)
        {
            _config.MinFrequency = minFrequency;
            _config.MaxFrequency = maxFrequency;
        }
    }

    // به روز رسانی فاصله زمانی
    public void UpdateInterval(int intervalMs)
    {
        if (_config != null)
        {
            _config.IntervalMs = intervalMs;
        }
    }
}
