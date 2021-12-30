using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Cko.PaymentGateway.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.UnitTests
{
    [TestFixture]
    public class PaymentPersistorTests
    {

        ICustomerRepository? _mockCustomerRepository = null;
        IPaymentCardRepository? _mockPaymentCardRepository = null;
        IMerchantRepository? _mockMerchantRepository = null;
        IPaymentRepository? _mockPaymentRepository = null;
        PaymentPersistor? _paymentPersistor = null;

        // Initialize mocks for each Test case
        [SetUp]
        public void MakePersistor()
        {
            _mockCustomerRepository = Substitute.For<ICustomerRepository>();
            _mockPaymentCardRepository = Substitute.For<IPaymentCardRepository>();
            _mockMerchantRepository = Substitute.For<IMerchantRepository>();
            _mockPaymentRepository = Substitute.For<IPaymentRepository>();

            _paymentPersistor = new PaymentPersistor(_mockCustomerRepository, _mockPaymentCardRepository, _mockMerchantRepository, _mockPaymentRepository);
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
            SaveCustomerDetails = true
        };

        [Test]
        public async Task Test_PaymentCardAndCustomerAreSetFromPaymentForNewCustoer()
        {

            var paymentRequest = MakeValidPayment();
            var customer = new Customer();
            var paymentCard = new PaymentCard();

            (customer,paymentCard) = await _paymentPersistor.SaveCustomerDetails(paymentRequest, customer, paymentCard);

            Assert.AreNotEqual(Guid.Empty, customer.CustomerReference);
            Assert.AreNotEqual(Guid.Empty, paymentCard.CustomerReference);

            Assert.AreEqual(paymentRequest.CustomerAddress, customer.CustomerAddress);
            Assert.AreEqual(paymentRequest.CustomerAddress, paymentCard.CustomerAddress);
            
            Assert.AreEqual(paymentRequest.CustomerName, customer.CustomerName);
            Assert.AreEqual(paymentRequest.CustomerName, paymentCard.CustomerName);

            Assert.AreEqual(paymentRequest.BankIdentifierCode, paymentCard.BankIdentifierCode);
            Assert.AreEqual(paymentRequest.CardExpiry, paymentCard.CardExpiry);
            Assert.AreEqual(paymentRequest.CardNumber, paymentCard.CardNumber);
            Assert.AreEqual(paymentRequest.Cvv, paymentCard.Cvv);

            await _mockCustomerRepository.Received().Insert(customer);
            await _mockPaymentCardRepository.Received().Insert(paymentCard);
        }


        [Test]
        public async Task Test_PaymentCardAndCustomerAreSetFromPaymentForExisting()
        {

            var paymentRequest = MakeValidPayment();

            var custRef = Guid.NewGuid();
            var customer = new Customer { CustomerReference = custRef };
            var paymentCard = new PaymentCard { CustomerReference = custRef };
            paymentCard.CustomerReference = custRef;

            (customer, paymentCard) = await _paymentPersistor.SaveCustomerDetails(paymentRequest, customer, paymentCard);

            Assert.AreEqual(custRef, customer.CustomerReference);
            Assert.AreEqual(custRef, paymentCard.CustomerReference);

            Assert.AreEqual(paymentRequest.CustomerAddress, customer.CustomerAddress);
            Assert.AreEqual(paymentRequest.CustomerAddress, paymentCard.CustomerAddress);

            Assert.AreEqual(paymentRequest.CustomerName, customer.CustomerName);
            Assert.AreEqual(paymentRequest.CustomerName, paymentCard.CustomerName);

            Assert.AreEqual(paymentRequest.BankIdentifierCode, paymentCard.BankIdentifierCode);
            Assert.AreEqual(paymentRequest.CardExpiry, paymentCard.CardExpiry);
            Assert.AreEqual(paymentRequest.CardNumber, paymentCard.CardNumber);
            Assert.AreEqual(paymentRequest.Cvv, paymentCard.Cvv);

            await _mockCustomerRepository.Received().Update(customer);
            await _mockPaymentCardRepository.Received().Update(paymentCard);
        }
    }
}
