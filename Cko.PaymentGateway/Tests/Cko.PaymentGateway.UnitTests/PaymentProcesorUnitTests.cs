using AutoMapper;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Cko.PaymentGateway.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Refit;

namespace Cko.PaymentGateway.UnitTests
{

    public class BankSdkUnitTest : IBankSdk
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
    public class PaymentProcesorUnitTests
    {

        private (
                    PaymentProcessor paymentProcessor,
                    ILogger<PaymentProcessor> mockLogger,
                    PaymentRepository mockPaymentRepo,
                    BankRepository mockBankRepo,
                    CustomerRepository mockCustomerRepo,
                    PaymentCardRepository mockPaymentCardRepo,
                    MerchantRepository mockMerchantRepo
                ) MakeTestPaymentProcessor()
        {
            var logger = Substitute.For<ILogger<PaymentProcessor>>();
            var paymentRepository = Substitute.For<PaymentRepository>();
            var bankRepository = Substitute.For<BankRepository>();
            var customerRepository = Substitute.For<CustomerRepository>();
            var paymentCardRepository = Substitute.For<PaymentCardRepository>();
            var merchantRepository = Substitute.For<MerchantRepository>();
            Func<string, IBankSdk> funcBank = (u) => new BankSdkUnitTest();
            var mockMapper = Substitute.For<IMapper>();

            return (
                    new PaymentProcessor(logger, paymentRepository, bankRepository, customerRepository, paymentCardRepository, merchantRepository,funcBank,mockMapper),
                    logger,
                    paymentRepository,
                    bankRepository,
                    customerRepository,
                    paymentCardRepository,
                    merchantRepository
                    );
        }

        [Test]
        public void Test_PaymentWithNoData()
        {

            var p = MakeTestPaymentProcessor();
            var processor = p.paymentProcessor;

            var paymentReq = new PaymentRequest();
            var (valid,message) = processor.IsValid(paymentReq);
            Assert.False(valid);
            Assert.IsNotEmpty(message);
        }

        [Test]
        public void Test_PayUsingValidCard_WithCardDetails_NoReference()
        {
            var p = MakeTestPaymentProcessor();
            var processor = p.paymentProcessor;

            var paymentReq = new PaymentRequest();

            paymentReq.CustomerName = "John";
            paymentReq.CustomerAddress = "1, Street, City, ABC123";
            //paymentReq.CustomerReference = Guid.NewGuid();

            paymentReq.CardNumber = "371449635398431";
            paymentReq.Cvv = "123";
            paymentReq.CardExpiry = DateTime.Today.AddYears(3);
            paymentReq.BankIdentifierCode = "Hsbc";

            paymentReq.ItemDetails = "An item to buy";
            paymentReq.Amount = 10;
            paymentReq.Ccy = "ABC";

            paymentReq.MerchantId = 10;
            paymentReq.MerchantName = "Amazon";

            var (valid, message) = processor.IsValid(paymentReq);
            Assert.True(valid);
            Assert.IsEmpty(message);
        }


        [Test]
        public void Test_PayUsingValidCard_WithCardDetails_WithReference()
        {
            var p = MakeTestPaymentProcessor();
            var processor = p.paymentProcessor;

            var paymentReq = new PaymentRequest();

            paymentReq.CustomerReference = Guid.NewGuid();

            paymentReq.ItemDetails = "An item to buy";
            paymentReq.Amount = 10;
            paymentReq.Ccy = "ABC";

            paymentReq.MerchantId = 10;
            paymentReq.MerchantName = "Amazon";

            var (valid, message) = processor.IsValid(paymentReq);
            Assert.True(valid);
            Assert.IsEmpty(message);
        }

    }
}