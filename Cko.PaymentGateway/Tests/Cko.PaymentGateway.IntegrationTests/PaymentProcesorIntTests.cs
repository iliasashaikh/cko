using Cko.PaymentGateway.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using System.Text.Json;

namespace Cko.PaymentGateway.IntegrationTests
{
    [TestFixture]
    public class PaymentProcesorIntTests
    {
        [Test]
        public void Test_PaymentDetailsAddedToDb()
        {
            var paymentReqString = @"{
  'customerName': 'John',
  'customerAddress': 'Smith',
  'customerReference': '00000000-0000-0000-0000-000000000000',
  'cardNumber': '5555555555554444',
  'cvv': '123',
  'cardExpiry': '2021-12-26T22:55:20.275Z',
  'bankIdentifierCode': 'Citi',
  'saveCustomerDetails': true,
  'itemDetails': 'something',
  'amount': 10,
  'ccy': 'usd',
  'merchantId': 1,
  'merchantName': 'Amazon'
}";
            var paymentReq = JsonSerializer.Deserialize<PaymentRequest>(paymentReqString);
        }

        [Test]
        public async Task Test_PaymentDetailsFetchedFromDb()
        {
            var application = new WebApplicationFactory<Program>()
                                .WithWebHostBuilder(builder =>
                                {
                                    // ... Configure test services
                                });

            var client = application.CreateClient();

            var response = await client.PostAsync("/ProcessPayments", null);
        }
    }
}