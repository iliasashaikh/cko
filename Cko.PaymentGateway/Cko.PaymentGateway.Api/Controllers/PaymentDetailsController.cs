using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cko.PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDetailsController : ControllerBase
    {
        private readonly ILogger<PaymentDetailsController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentDetailsController(ILogger<PaymentDetailsController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

        [HttpGet(Name = "GetPaymentDetails")]
        public IEnumerable<WeatherForecast> Get(string paymentReference)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
            })
            .ToArray();
        }
    }
}
