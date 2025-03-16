using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace SignalReceiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalReceiverController : ControllerBase
    {
        private readonly SignalReceiverService _signalReceiverService;

        public SignalReceiverController(SignalReceiverService signalReceiverService)
        {
            _signalReceiverService = signalReceiverService;
        }

        [HttpGet("receiveSignal")]
        public async Task<IActionResult> ReceiveSignal()
        {
            await _signalReceiverService.ReceiveSignalFromModbus();
            return Ok("Signal received successfully.");
        }
    }
}
