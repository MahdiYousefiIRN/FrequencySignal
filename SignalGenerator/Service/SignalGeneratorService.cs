using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Modbus;
using SignalGenerator.Models;
using SignalGenerator.Services;
using System.Text;
using System.Text.Json;

public class SignalGeneratorService
{
    private const string RequestUriAPI = "http://localhost:5002/api/packetdata";
    private readonly IHubContext<SignalHub> _hubContext;
    private readonly ModbusClientManager _modbusClientManager;
    private readonly ILogger<SignalGeneratorService> _logger;
    private readonly HttpClient _httpClient;
    private SignalConfigGeneration? _config;
    private CancellationTokenSource? _cancellationTokenSource;
    private Timer? _signalGenerationTimer;

    public SignalGeneratorService(
        IHubContext<SignalHub> hubContext,
        ModbusClientManager modbusClientManager,
        ILogger<SignalGeneratorService> logger,
        HttpClient httpClient)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _modbusClientManager = modbusClientManager ?? throw new ArgumentNullException(nameof(modbusClientManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> StartSignalGeneration(SignalConfigGeneration signalConfigGeneration, CancellationToken cancellationToken)
    {
        if (signalConfigGeneration == null)
            throw new ArgumentNullException(nameof(signalConfigGeneration));

        // اگر تولید سیگنال‌ها قبلاً در حال اجراست، آن را متوقف کنید
        StopSignalGeneration();

        // به‌روزرسانی تنظیمات جدید
        _config = signalConfigGeneration;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // فعال کردن تایمر برای تولید سیگنال‌ها به‌طور دوره‌ای
        _signalGenerationTimer = new Timer(async _ =>
        {
            // بررسی لغو شدن درخواست و توقف تولید سیگنال‌ها
            if (_cancellationTokenSource?.Token.IsCancellationRequested == true)
            {
                StopSignalGeneration();
                return;
            }

            await GenerateSignals(_cancellationTokenSource?.Token ?? CancellationToken.None);
        }, null, 0, _config.IntervalMs);

        return "Signal generation started successfully.";
    }

    private async Task GenerateSignals(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        try
        {
            _logger.LogInformation("Generating signals...");

            var signalData = GenerateSignalData();

            // اجرای ارسال سیگنال‌ها به صورت همزمان
            var tasks = new List<Task>
            {
                SendSignalToSignalR(signalData),
                SendSignalToModbus(signalData),
                SendSignalToHttp(signalData)
            };

            await Task.WhenAll(tasks);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Signal generation task canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in signal generation: {ex.Message}");
        }
    }

    private List<double> GenerateSignalData()
    {
        var random = new Random();
        var signalData = new List<double>();

        for (int i = 0; i < _config.SignalCount; i++)
        {
            double frequency = random.NextDouble() * (_config.MaxFrequency - _config.MinFrequency) + _config.MinFrequency;
            signalData.Add(frequency);
        }

        return signalData;
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
            int[] modbusData = signalData.Select(d => (int)(d * 1000)).ToArray();

            if (!_modbusClientManager.IsConnected)
            {
                _modbusClientManager.Connect();
            }

            _modbusClientManager.WriteModbusData(0, modbusData);
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
            // تبدیل داده‌های سیگنال به JSON
            var jsonContent = JsonSerializer.Serialize(new
            {
                SignalData = signalData,  // داده‌های سیگنال به صورت لیست
                Timestamp = DateTime.Now   // زمان ارسال
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // ارسال داده به API پروژه دوم
            var response = await _httpClient.PostAsync(RequestUriAPI, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Signal sent to HTTP API successfully.");
            }
            else
            {
                _logger.LogError($"Failed to send signal to HTTP API. Status Code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending signal to HTTP API: {ex.Message}");
        }
    }

    public void StopSignalGeneration()
    {
        if (_signalGenerationTimer != null)
        {
            _signalGenerationTimer.Change(Timeout.Infinite, Timeout.Infinite);  // متوقف کردن تایمر
            _signalGenerationTimer.Dispose();
        }
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _logger.LogInformation("Signal generation stopped.");
    }

    public void UpdateInterval(int newInterval)
    {
        if (_config is not null)
        {
            _config.IntervalMs = newInterval;
            _signalGenerationTimer?.Change(0, _config.IntervalMs); // تغییر زمان تایمر
            _logger.LogInformation($"Signal generation interval updated to {newInterval} ms.");
        }
    }
}
