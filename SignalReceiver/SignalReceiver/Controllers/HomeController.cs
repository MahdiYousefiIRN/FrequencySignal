using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SignalReceiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignalReceiverService _signalReceiverService;

        public HomeController(SignalReceiverService signalReceiverService)
        {
            _signalReceiverService = signalReceiverService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // اکشن برای دریافت سیگنال‌ها
        public async Task<IActionResult> SignalReceiver()
        {
            // دریافت سیگنال‌ها از SignalReceiverService
            var signals = await _signalReceiverService.GetSignalFromApi();  // یا از Modbus یا SignalR استفاده کنید

            return View(signals);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
