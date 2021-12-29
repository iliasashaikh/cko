using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Refit;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.IntegrationTests2
{
    public class BankSdkIntTest : IBankSdk
    {
        public async Task<BankPaymentResponse> ProcessPayment([Body] BankPaymentRequest paymentRequest)
        {
            var response = new BankPaymentResponse();

            response.BankPaymentResponseMessage = "Test";
            response.PaymentReference = Guid.NewGuid();
            response.BankReponseCode = 0;

            return await Task.Run(() => response);
        }
    }

    [TestFixture]
    public class PaymentControllerDbPersistenceTests
    {
        private WebApplicationFactory<Program> _application;
        private HttpClient _client;

        private MerchantRepository _merchantRepo;
        private PaymentRepository _paymentRepo;
        private CustomerRepository _customerRepo;
        private BankRepository _bankRepo;
        private PaymentCardRepository _paymentCardRepo;

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

                                        builder.ConfigureTestServices(services =>
                                        {
                                            services.AddScoped<Func<string, IBankSdk>>(s => (u) => new BankSdkIntTest());
                                        });

                                    }
                                );

            _client = _application.CreateClient();
            _merchantRepo = (MerchantRepository)_application.Services.GetService(typeof(MerchantRepository));
            _paymentRepo = (PaymentRepository)_application.Services.GetService(typeof(PaymentRepository));
            _customerRepo = (CustomerRepository)_application.Services.GetService(typeof(CustomerRepository));
            _bankRepo = (BankRepository)_application.Services.GetService(typeof(BankRepository));
            _paymentCardRepo = (PaymentCardRepository)_application.Services.GetService(typeof(PaymentCardRepository));

        }

        [SetUp]
        public async Task ResetDb()
        {
            await _merchantRepo.Run("Truncate table Merchant");
            await _paymentRepo.Run("Truncate table Payment");
            await _customerRepo.Run("Truncate table Customer");
            await _bankRepo.Run("Truncate table Bank");
            await _paymentCardRepo.Run("Truncate table PaymentCard");
        }

        [Test]
        public async Task Test_PaymentWithEmptyPaymentRequest()
        {
            var paymentReq = new PaymentRequest();
            var response = await _client.PostAsJsonAsync("api/Payments", paymentReq);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Test_PaymentWithValidMerchant_NoPaymentDetails()
        {
            var id = await _merchantRepo.Insert(new Entities.Merchant() { Name = "a", Address = "address 2" });

            var paymentReq = new PaymentRequest() { MerchantId = id };
            var response = await _client.PostAsJsonAsync("api/Payments", paymentReq);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }


        [Test]
        public async Task Test_PaymentWithValidMerchant_ValidPaymentDetails_WithCustomerReference()
        {
            var custRef = Guid.NewGuid();

            var id = await _merchantRepo.Insert(new Entities.Merchant() { Name = "Amazon", Address = "The Amazon" });
            var custId = await _customerRepo.Insert(new Entities.Customer() { CustomerName = "John Smith", CustomerAddress = "1 street", CustomerReference = custRef });
            var paymentCardId = await _paymentCardRepo.Insert(new Entities.PaymentCard { BankIdentifierCode = "hsbc", CardExpiry = DateTime.Today.AddYears(3), CardNumber = "5555555555554444", CustomerReference = custRef, CustomerName = "John Smith", CustomerAddress = "1 street", Cvv = "123" });
            var bankId = await _bankRepo.Insert(new Entities.Bank { BankName = "hsbc",BankIdentifier = "hsbc", BankApiUrl="apiurl"});

            var paymentReq = new PaymentRequest()
            {
                MerchantId = id,
                CustomerReference = custRef,
                Amount = 10,
                Ccy = "GBP",
                PaymentTime = DateTime.Today,
            };
            var response = await _client.PostAsJsonAsync("api/Payments", paymentReq);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task Test_PaymentWithValidMerchant_ValidPaymentDetails_WithoutCustomerReference()
        {
            var custRef = Guid.Empty;
            var merchantId = await _merchantRepo.Insert(new Entities.Merchant() { Name = "Amazon", Address = "The Amazon" });
            var bankId = await _bankRepo.Insert(new Entities.Bank { BankName = "hsbc", BankIdentifier = "hsbc", BankApiUrl = "apiurl" });

            var paymentReq = new PaymentRequest()
            {
                MerchantId = merchantId,
                CustomerReference = custRef,
                Amount = 10,
                Ccy = "GBP",
                PaymentTime = DateTime.Today,
                CardExpiry = DateTime.Today.AddYears(3),
                CustomerName = "John Smith",
                CustomerAddress = "1 street",
                CardNumber = "5555555555554444",
                Cvv = "123",
                BankIdentifierCode = "hsbc"

            };
            var response = await _client.PostAsJsonAsync("api/Payments", paymentReq);
            var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var paymentReadback = await _client.GetFromJsonAsync<Payment>($"api/payments/{paymentResponse.PaymentId}");
            Assert.IsNotNull(paymentReadback);  
        }
    }
}