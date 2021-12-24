using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cko.PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProcessPaymentsController : ControllerBase
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public ProcessPaymentsController(IPaymentProcessor paymentProcessor)
        {
            this._paymentProcessor = paymentProcessor;
        }

        [HttpPost(Name = "ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(PaymentRequest paymentRequest)
        {
            var response = await _paymentProcessor.ProcessPayment(paymentRequest);
            return Ok(response);
        }
    }
}
