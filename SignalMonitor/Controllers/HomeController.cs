using Microsoft.AspNetCore.Mvc;
using SignalMonitor.Data;
using SignalMonitor.Models;
using System.Linq;

namespace SignalMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // اکشن برای نمایش صفحه اصلی
        public IActionResult Index()
        {
            return View();
        }

        // اکشن برای نمایش لیست داده‌های SignalData از پایگاه داده
        public IActionResult PacketDataList()
        {
            var packetDataList = _context.SignalData.ToList(); // دریافت داده‌ها از پایگاه داده
            return View(packetDataList); // ارسال داده‌ها به ویو
        }
    }
}
