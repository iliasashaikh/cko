using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;

namespace Cko.PaymentGateway.Services
{
    public class PaymentPersistor : IPaymentPersistor
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentCardRepository _paymentCardRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentPersistor(ICustomerRepository customerRepository,
                                IPaymentCardRepository paymentCardRepository,
                                IMerchantRepository merchantRepository,
                                IPaymentRepository paymentRepository)
        {
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._paymentRepository = paymentRepository;
        }

        public async Task<(Customer,PaymentCard)> SaveCustomerDetails(PaymentRequest paymentRequest, Customer customer, PaymentCard paymentCard)
        {
            // fill in customer info from the payment Request
            paymentCard.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
            paymentCard.CardNumber = string.IsNullOrWhiteSpace(paymentRequest.CardNumber) ? paymentCard.CardNumber : paymentRequest.CardNumber;
            paymentCard.CustomerAddress = string.IsNullOrWhiteSpace(paymentRequest.CustomerAddress) ? paymentCard.CustomerAddress : paymentRequest.CustomerAddress;
            paymentCard.CustomerName = string.IsNullOrWhiteSpace(paymentRequest.CustomerName) ? paymentCard.CustomerName : paymentRequest.CustomerName;
            paymentCard.BankIdentifierCode = string.IsNullOrWhiteSpace(paymentRequest.BankIdentifierCode) ? paymentCard.BankIdentifierCode : paymentRequest.BankIdentifierCode;
            paymentCard.Cvv = string.IsNullOrWhiteSpace(paymentRequest.Cvv) ? paymentCard.Cvv : paymentRequest.Cvv;


            customer.CustomerName = paymentRequest.CustomerName == default(string) ? customer.CustomerName : paymentRequest.CustomerName;
            customer.CustomerAddress = paymentRequest.CustomerAddress == default(string) ? customer.CustomerAddress : paymentRequest.CustomerAddress;

            var custRef = Guid.NewGuid();
            if (paymentRequest.CustomerReference == Guid.Empty || customer.CustomerReference == Guid.Empty)
            {
                customer.CustomerReference = custRef;
                paymentCard.CustomerReference = custRef;
            }

            if (customer.CustomerReference == custRef)
            { 
                await _customerRepository.Insert(customer);
                await _paymentCardRepository.Insert(paymentCard);
            }
            else
            {
                await _customerRepository.Update(customer);
                await _paymentCardRepository.Update(paymentCard);
            }

            return (customer, paymentCard);

        }

        public async Task UpdatePaymentState(Payment payment, PaymentState state, string info)
        {
            (payment.State, payment.PaymentInfo) = (state, info);
            _ = await _paymentRepository.Update(payment);
        }

        public async Task<int> SavePayment(Payment payment)
        {
            if (payment.PaymentId == 0)
                _ = await _paymentRepository.Insert(payment);
            else
                _ = await _paymentRepository.Update(payment);

            return payment.PaymentId;
        }



    }
}
