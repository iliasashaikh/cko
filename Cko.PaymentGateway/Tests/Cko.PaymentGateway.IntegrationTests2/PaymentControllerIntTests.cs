using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;

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
            _application = new WebApplicationFactory<Program>()
                                .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((c, b) =>
                {
                    var p = b.Properties;
                    var projectDir = Directory.GetCurrentDirectory();
                    var configPath = Path.Combine(projectDir, "appsettings.json");

                    b.AddJsonFile(configPath);
                });
            });

            _client = _application.CreateClient();
            _merchantRepo = (MerchantRepository) _application.Services.GetService(typeof(MerchantRepository));

        }

        [Test]
        public async Task Test_PaymentWithEmptyPaymentRequest()
        {
            var paymentReq = new PaymentRequest();
            var response = await _client.PostAsJsonAsync("api/Payments",paymentReq);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Test]
        public async Task Test_PaymentWithValidMerchant()
        {
            var id = await _merchantRepo.Insert(new Entities.Merchant() {Name="a", Address="address 2" });

            var paymentReq = new PaymentRequest() { MerchantId = id };
            var response = await _client.PostAsJsonAsync("api/Payments", paymentReq);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}