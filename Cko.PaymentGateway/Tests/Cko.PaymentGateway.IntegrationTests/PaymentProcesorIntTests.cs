using Cko.PaymentGateway.Models;
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
            var paymentReq = @"{
  'customerName': 'John',
  'customerAddress': 'Smith',
  'customerReference': '00000000-0000-0000-0000-000000000000',
  'cardNumber': '5555555555554444',
  'cvv': '123',
  'cardExpiry': '2021-12-26T22:55:20.275Z',
  'bankIdentifierCode': 'hsbc',
  'saveCustomerDetails': true,
  'itemDetails': 'sofa',
  'amount': 10,
  'ccy': 'usd',
  'merchantId': 1,
  'merchantName': 'string'
}";
            
        }

        [Test]
        public void Test_PaymentDetailsFetchedFromDb()
        {

        }

    }
}