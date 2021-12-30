using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Cko.PaymentGateway.Services;
using NSubstitute;
using NUnit.Framework;

namespace Cko.PaymentGateway.UnitTests
{
    [TestFixture]
    public class PaymentValidatorTests
    {

        ICustomerRepository _mockCustomerRepository = null;
        IPaymentCardRepository _mockPaymentCardRepository = null;
        IMerchantRepository _mockMerchantRepository = null;
        IPaymentRepository _mockPaymentRepository = null;
        PaymentPersistor _paymentPersistor = null;
        PaymentValidator _paymentValidator = null;

        // Initialize mocks for each Test case
        [SetUp]
        public void MakeValidator()
        {
            _mockCustomerRepository = Substitute.For<ICustomerRepository>();
            _mockPaymentCardRepository = Substitute.For<IPaymentCardRepository>();
            _mockMerchantRepository = Substitute.For<IMerchantRepository>();
            _mockPaymentRepository = Substitute.For<IPaymentRepository>();

            _paymentPersistor = new PaymentPersistor(_mockCustomerRepository, _mockPaymentCardRepository, _mockMerchantRepository, _mockPaymentRepository);
            _paymentValidator = new PaymentValidator(_mockCustomerRepository, _mockPaymentCardRepository, _mockMerchantRepository, _paymentPersistor);
        }

        [Test]
        public async Task Test_MerchantIdIsMissing_ReturnInvalid()
        {
            var paymentRequest = new PaymentRequest();
            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = await _paymentValidator.IsValidMerchant(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }


        [Test]
        public async Task Test_MerchantIdIsPresent_ReturnValid()
        {
            var paymentRequest = new PaymentRequest();
            var paymentResponse = new PaymentResponse();
            _mockMerchantRepository.GetMerchantByIdentifier(0).Returns(Task.FromResult(new Merchant()));

            (var valid, paymentResponse) = await _paymentValidator.IsValidMerchant(paymentRequest, paymentResponse);

            Assert.IsTrue(valid);
            Assert.IsEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public async Task Test_CustomerIsMissing_ReturnInValid()
        {
            // if a customer reference is provided ,we should have the details for that in our db
            var paymentRequest = new PaymentRequest { CustomerReference = Guid.NewGuid() };

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse, var customer, var paymentCard) = await _paymentValidator.IsValidCustomer(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }


        [Test]
        public async Task Test_CustomerPresentButPaymentCardIsMissing_ReturnInValid()
        {
            // if a customer reference is provided ,we should have the details for that in our db
            var customerReference = Guid.NewGuid();

            var paymentRequest = new PaymentRequest { CustomerReference = customerReference };
            var paymentResponse = new PaymentResponse();

            _mockCustomerRepository.GetCustomerByReference(customerReference).Returns(Task.FromResult(new Customer { CustomerReference = customerReference }));
            (var valid, paymentResponse, var customer, var paymentCard) = await _paymentValidator.IsValidCustomer(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }


        [Test]
        public async Task Test_CustomerIsMissingPaymentCardPresent_ReturnInValid()
        {
            var customerReference = Guid.NewGuid();

            var paymentRequest = new PaymentRequest { CustomerReference = customerReference };
            var paymentResponse = new PaymentResponse();

            _mockPaymentCardRepository.GetPaymentCardByCustomerReference(customerReference).Returns(Task.FromResult(new PaymentCard { CustomerReference = customerReference }));
            (var valid, paymentResponse, var customer, var paymentCard) = await _paymentValidator.IsValidCustomer(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public async Task Test_CustomerAndPaymentCardArePresent_ReturnValid()
        {
            var customerReference = Guid.NewGuid();

            var paymentRequest = new PaymentRequest { CustomerReference = customerReference };
            var paymentResponse = new PaymentResponse();

            _mockPaymentCardRepository.GetPaymentCardByCustomerReference(customerReference).Returns(Task.FromResult(new PaymentCard { CustomerReference = customerReference }));
            _mockCustomerRepository.GetCustomerByReference(customerReference).Returns(Task.FromResult(new Customer { CustomerReference = customerReference }));

            (var valid, paymentResponse, var customer, var paymentCard) = await _paymentValidator.IsValidCustomer(paymentRequest, paymentResponse);

            Assert.IsTrue(valid);
            Assert.IsEmpty(paymentResponse.PaymentResponseMessage);
        }

        PaymentRequest MakeValidPayment() => new PaymentRequest
        {

            CustomerName = "John",
            CustomerAddress = "1, Street, City, ABC123",
            CustomerReference = Guid.NewGuid(),
            CardNumber = "371449635398431",
            Cvv = "123",
            CardExpiry = DateTime.Today.AddYears(3),
            BankIdentifierCode = "Hsbc",
            ItemDetails = "An item to buy",
            Amount = 10,
            Ccy = "ABC",
            MerchantId = 10,
            MerchantName = "Amazon",
        };

        [Test]
        public void Test_ValidCard_ReturnValid()
        {

            var paymentRequest = MakeValidPayment();
            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            TestContext.WriteLine(paymentResponse.PaymentResponseMessage);

            Assert.True(valid);
            Assert.IsEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_InvalidCardNumber_ReturnInValid()
        {

            var paymentRequest = MakeValidPayment();
            paymentRequest.CustomerReference = Guid.Empty;
            paymentRequest.CardNumber = "";
            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_ExpiredCard_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.CardExpiry = DateTime.Today.AddDays(-1);

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_MissingCvvNoCustomerReference_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.CustomerReference = Guid.Empty;
            paymentRequest.Cvv = "";

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_WithCustomerReference_ReturnValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.Cvv = "";

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsTrue(valid);
            Assert.IsEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_MissingCurrency_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.CustomerReference = Guid.Empty;
            paymentRequest.Ccy = "";

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public void Test_MissingAmount_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.Amount = 0;

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public async Task Test_NegativeAmount_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.Amount = -1;

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }

        [Test]
        public async Task Test_MissingBankIdentifier_ReturnInValid()
        {
            var paymentRequest = MakeValidPayment();
            paymentRequest.CustomerReference = Guid.Empty;
            paymentRequest.BankIdentifierCode = "";

            var paymentResponse = new PaymentResponse();

            (var valid, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            Assert.IsFalse(valid);
            Assert.IsNotEmpty(paymentResponse.PaymentResponseMessage);
        }


    }
}
