using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cko.PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentsController(IPaymentProcessor paymentProcessor)
        {
            this._paymentProcessor = paymentProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(PaymentRequest paymentRequest)
        {
            try
            {
                var response = await _paymentProcessor.ProcessPayment(paymentRequest);
                switch (response.Status)
                {
                    case PaymentResponseStatus.Approved:
                        return Ok(response);

                    case PaymentResponseStatus.Rejected_MerchantNotFound:
                        return NotFound(response);

                    case PaymentResponseStatus.Bank_NotFound:
                        return NotFound(response);

                    case PaymentResponseStatus.Rejected_CardValidationFailed:
                        return UnprocessableEntity(response);

                    case PaymentResponseStatus.Rejected_DeclinedByBank:
                        return UnprocessableEntity(response);
                    default:
                        return BadRequest(response);
                }
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        [HttpGet("{id}")]   // GET /api/Payments/xyz
        public IActionResult GetPayment(string id)
        {
            return Ok(id);
        }
    }
}
