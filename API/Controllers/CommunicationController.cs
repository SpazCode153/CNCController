using Microsoft.AspNetCore.Mvc;
using API.Machines;
using API.Requests;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunicationController: ControllerBase
    {
        private MarlinMachineHandler marlinMachineHandler;

        public CommunicationController(MarlinMachineHandler marlinMachineHandler)
        {
            this.marlinMachineHandler = marlinMachineHandler;
        }

        [HttpPost]
        [Route("Set")]
        public IActionResult Set(CommunicationPost request)
        {
            this.marlinMachineHandler.SetCommunicatonSettings(request.ComPort,
                request.BaudRate,
                request.Parity,
                request.Databits,
                request.Stopbits);

            return Ok();
        }


        [HttpGet]
        [Route("Connect")]
        public IActionResult Connect()
        {
            this.marlinMachineHandler.Start();

            return Ok();
        }

        [HttpGet]
        [Route("Disconnect")]
        public IActionResult Disconnect()
        {
            this.marlinMachineHandler.Stop();

            return Ok();
        }
    }
}
