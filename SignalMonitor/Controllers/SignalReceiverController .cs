using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalMonitor.Data;
using SignalMonitor.Models;
using SignalMonitor.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace SignalMonitor.Controllers
{
    public class SignalReceiverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalHub> _hubContext;

        public SignalReceiverController(ApplicationDbContext context, IHubContext<SignalHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] dynamic signalData)
        {
            try
            {
                // تبدیل داده‌های دریافتی به مدل SignalData
                var SignalData = new SignalData
                {
                    // اختصاص مقادیر به ویژگی‌های مدل از داده‌های دریافتی
                    SourceIP = signalData.SourceIP,
                    DestinationIP = signalData.DestinationIP,
                    Protocol = signalData.Protocol,
                    Value = signalData.Value,
                    IsMalicious = signalData.IsMalicious,
                    Status = signalData.Status,
                    SecurityKey = signalData.SecurityKey,
                    ReceivedTimestamp = DateTime.UtcNow,  // استفاده از زمان کنونی
                };

                // ذخیره داده‌ها در پایگاه داده
                _context.SignalData.Add(SignalData);
                await _context.SaveChangesAsync();

                // ارسال داده به کلاینت‌ها از طریق SignalR (در صورت نیاز)
                await _hubContext.Clients.All.SendAsync("ReceiveSignalData", SignalData);

                return Ok();
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا، اطلاعات خطا را ثبت می‌کنیم
                return BadRequest($"Error processing signal data: {ex.Message}");
            }
        }


        // اکشن برای نمایش داده‌های SignalData
        public IActionResult Index()
        {
            var signalDataList = _context.SignalData.ToList(); // دریافت داده‌ها از پایگاه داده
            return View(signalDataList); // ارسال داده‌ها به ویو
        }
    }
}
