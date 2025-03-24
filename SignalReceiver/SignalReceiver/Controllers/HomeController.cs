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
       

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
