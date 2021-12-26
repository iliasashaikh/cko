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
            try
            {
                var response = await _paymentProcessor.ProcessPayment(paymentRequest);
                switch (response.Status)
                {
                    case PaymentResponseStatus.Approved:
                        return Ok(response);

                    case PaymentResponseStatus.Rejected_MerchantNotFound:
                        return NotFound(response);

                    case PaymentResponseStatus.Rejected_CustomerNotFound:
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
    }
}
