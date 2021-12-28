using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.IntegrationTests2
{
    [TestFixture]
    public class PaymentControllerIntTests
    {
        private WebApplicationFactory<Program> _application;
        private HttpClient _client;
        private MerchantRepository _merchantRepo;

        [OneTimeSetUp]
        public void CreateTestFactory()
        {
            _application = new WebApplicationFactory<Program>();
            _client = _application.WithWebHostBuilder(builder => 
            {
                builder.ConfigureAppConfiguration
            });

            _merchantRepo = (MerchantRepository) _application.Services.GetService(typeof(MerchantRepository));

        }

        [Test]
        public async Task Test_PaymentWithEmptyPaymentRequest()
        {
            var paymentReq = new PaymentRequest();
            var response = await _client.PostAsJsonAsync("api/Payments",paymentReq);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}