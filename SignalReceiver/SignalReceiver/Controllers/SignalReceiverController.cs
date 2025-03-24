using Microsoft.AspNetCore.Mvc;
using SignalReceiver.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalReceiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalReceiverController : ControllerBase
    {
        private readonly SignalReceiverService _signalReceiverService;

        // سازنده کنترلر برای دریافت SignalReceiverService
        public SignalReceiverController(SignalReceiverService signalReceiverService)
        {
            _signalReceiverService = signalReceiverService;
        }

        // اکشن برای دریافت سیگنال‌ها از Modbus
        [HttpPost("receiveSignalModbus")]
        public async Task<IActionResult> ReceiveSignalFromModbus()
        {
            // دریافت سیگنال‌ها از Modbus
            List<double> signals = await _signalReceiverService.ReceiveSignalFromModbus();

            if (signals.Count > 0)
            {
                return Ok(new { message = "Signal received from Modbus successfully.", signals });
            }
            else
            {
                return BadRequest("No signal received from Modbus.");
            }
        }

        // اکشن برای دریافت سیگنال‌ها از SignalR
        [HttpPost("receiveSignalSignalR")]
        public async Task<IActionResult> ReceiveSignalFromSignalR()
        {
            // شروع اتصال به SignalR (اگر هنوز شروع نشده باشد)
            await _signalReceiverService.StartReceivingSignalFromSignalRAsync();

            // دریافت سیگنال‌ها از SignalR
            var signals = _signalReceiverService.GetSignalData(); // سیگنال‌ها قبلاً از SignalR دریافت شده‌اند

            if (signals.Count > 0)
            {
                return Ok(new { message = "Signal received from SignalR successfully.", signals });
            }
            else
            {
                return BadRequest("No signal received from SignalR.");
            }
        }

        // اکشن برای دریافت سیگنال‌ها از API
        [HttpPost("receiveSignalAPI")]
        public async Task<IActionResult> ReceiveSignalFromApi([FromBody] SignalRequestDto request)
        {
            if (request == null || request.SignalData == null || request.SignalData.Count == 0)
            {
                return BadRequest("Invalid signal data received.");
            }
            var signals = await _signalReceiverService.GetSignalFromApi(request);  // یا از Modbus یا SignalR استفاده کنید

            //// ذخیره داده‌ها در دیتابیس
            //await _signalReceiverService.SaveSignalsToDatabase(request.SignalData, request.Timestamp);

            return Ok(new { message = "Signal received and stored successfully.", signals = request.SignalData });
        }

    }
}
