using Microsoft.AspNetCore.SignalR;
using SignalGenerator.Modbus;
using SignalGenerator.Models;
using SignalGenerator.Services;
using System.Text;
using System.Text.Json;

public class SignalGeneratorService
{
    // آدرس API برای ارسال داده‌ها
    private const string RequestUriAPI = "http://localhost:5002/api/packetdata";

    // وابستگی‌ها
    private readonly IHubContext<SignalHub> _hubContext;
    private readonly ModbusClientManager _modbusClientManager;
    private readonly ILogger<SignalGeneratorService> _logger;
    private readonly HttpClient _httpClient;

    // تنظیمات و متغیرهای داخلی
    private SignalConfigGeneration? _config;
    private CancellationTokenSource? _cancellationTokenSource;
    private Timer? _signalGenerationTimer;

    // سازنده کلاس که وابستگی‌ها را دریافت می‌کند
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

    // متد شروع تولید سیگنال
    public async Task<string> StartSignalGeneration(SignalConfigGeneration signalConfigGeneration, CancellationToken cancellationToken)
    {
        if (signalConfigGeneration == null)
            throw new ArgumentNullException(nameof(signalConfigGeneration));

        // در صورتی که تولید سیگنال‌ها قبلاً در حال اجراست، آن را متوقف می‌کنیم
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

            // تولید سیگنال‌ها
            await GenerateSignals(_cancellationTokenSource?.Token ?? CancellationToken.None);
        }, null, 0, _config.IntervalMs);

        return "Signal generation started successfully.";
    }

    // متد تولید سیگنال‌ها
    private async Task GenerateSignals(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        try
        {
            // ورود به حالت تولید سیگنال‌ها
            _logger.LogInformation("Generating signals...");

            // داده‌های سیگنال تولید شده
            var signalData = GenerateSignalData();

            // ارسال سیگنال‌ها به روش‌های مختلف
            var tasks = new List<Task>
            {
                SendSignalToSignalR(signalData),  // ارسال به SignalR
                SendSignalToModbus(signalData),   // ارسال به Modbus
                SendSignalToHttp(signalData)      // ارسال به API
            };

            // منتظر انجام تمامی درخواست‌ها به طور همزمان
            await Task.WhenAll(tasks);
        }
        catch (TaskCanceledException)
        {
            // در صورتی که درخواست لغو شود
            _logger.LogInformation("Signal generation task canceled.");
        }
        catch (Exception ex)
        {
            // در صورت وقوع خطا
            _logger.LogError($"Error in signal generation: {ex.Message}");
        }
    }

    // متد تولید داده‌های سیگنال به صورت تصادفی
    private List<double> GenerateSignalData()
    {
        var random = new Random();
        var signalData = new List<double>();

        // تولید داده‌های تصادفی برای سیگنال‌ها
        for (int i = 0; i < _config.SignalCount; i++)
        {
            double frequency = random.NextDouble() * (_config.MaxFrequency - _config.MinFrequency) + _config.MinFrequency;
            signalData.Add(frequency);
        }

        return signalData;
    }

    // متد ارسال سیگنال به SignalR
    private async Task SendSignalToSignalR(List<double> signalData)
    {
        try
        {
            // ارسال داده‌ها به تمام کلاینت‌ها از طریق SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSignalData", signalData);
            _logger.LogInformation("Signal sent to SignalR clients.");
        }
        catch (Exception ex)
        {
            // در صورت بروز خطا در ارسال سیگنال
            _logger.LogError($"Error sending signal to SignalR: {ex.Message}");
        }
    }

    // متد ارسال سیگنال به Modbus
    private async Task SendSignalToModbus(List<double> signalData)
    {
        try
        {
            // تبدیل داده‌های سیگنال به مقادیر صحیح برای Modbus
            int[] modbusData = signalData.Select(d => (int)(d * 1000)).ToArray();

            // بررسی وضعیت اتصال به Modbus و اتصال در صورت عدم اتصال
            if (!_modbusClientManager.IsConnected)
            {
                _modbusClientManager.Connect();
            }

            // ارسال داده‌ها به Modbus
            _modbusClientManager.WriteModbusData(0, modbusData);
            _logger.LogInformation("Signal sent to Modbus.");
        }
        catch (Exception ex)
        {
            // در صورت بروز خطا در ارسال سیگنال
            _logger.LogError($"Error sending signal to Modbus: {ex.Message}");
        }
    }

    // متد ارسال سیگنال به API (HTTP)
    private async Task SendSignalToHttp(List<double> signalData)
    {
        try
        {
            // تبدیل داده‌های سیگنال به JSON
            var jsonContent = JsonSerializer.Serialize(new
            {
                SignalData = signalData,
                Timestamp = DateTime.Now   // زمان ارسال سیگنال
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
            // در صورت بروز خطا در ارسال سیگنال
            _logger.LogError($"Error sending signal to HTTP API: {ex.Message}");
        }
    }

    // متد متوقف کردن تولید سیگنال‌ها
    public void StopSignalGeneration()
    {
        // توقف تایمر تولید سیگنال
        if (_signalGenerationTimer != null)
        {
            _signalGenerationTimer.Change(Timeout.Infinite, Timeout.Infinite);  // متوقف کردن تایمر
            _signalGenerationTimer.Dispose();
        }

        // لغو فرآیندهای در حال اجرا
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _logger.LogInformation("Signal generation stopped.");
    }

    // متد به‌روزرسانی فاصله زمانی تولید سیگنال‌ها
    public void UpdateInterval(int newInterval)
    {
        if (_config is not null)
        {
            // به‌روزرسانی فاصله زمانی تولید سیگنال‌ها
            _config.IntervalMs = newInterval;
            _signalGenerationTimer?.Change(0, _config.IntervalMs); // تغییر زمان تایمر
            _logger.LogInformation($"Signal generation interval updated to {newInterval} ms.");
        }
    }
}
