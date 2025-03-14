using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalMonitor.Data;
using SignalMonitor.Models;
using SignalMonitor.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace SignalMonitor.Controllers
{
    public class PacketDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<PacketDataHub> _hubContext;

        public PacketDataController(ApplicationDbContext context, IHubContext<PacketDataHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // اکشن برای دریافت داده‌ها و ذخیره آن‌ها
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PacketData packetData)
        {
            // ذخیره داده در پایگاه داده
            _context.PacketData.Add(packetData);
            await _context.SaveChangesAsync();

            // ارسال داده به کلاینت‌ها از طریق SignalR
            await _hubContext.Clients.All.SendAsync("ReceivePacketData", packetData);

            return Ok();
        }

        // اکشن برای نمایش داده‌های PacketData
        public IActionResult Index()
        {
            var packetDataList = _context.PacketData.ToList(); // دریافت داده‌ها از پایگاه داده
            return View(packetDataList); // ارسال داده‌ها به ویو
        }
    }
}
