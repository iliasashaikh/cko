using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;

namespace Cko.PaymentGateway.Services
{
    public class PaymentPersistor
    {
        private readonly CustomerRepository _customerRepository;
        private readonly PaymentCardRepository _paymentCardRepository;
        private readonly MerchantRepository _merchantRepository;
        private readonly PaymentRepository _paymentRepository;

        public PaymentPersistor(CustomerRepository customerRepository,
                                PaymentCardRepository paymentCardRepository,
                                MerchantRepository merchantRepository,
                                PaymentRepository paymentRepository)
        {
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._paymentRepository = paymentRepository;
        }

        public async Task SaveCustomerDetails(PaymentRequest paymentRequest, Customer customer, PaymentCard paymentCard)
        {
            // fill in customer info from the payment Request
            paymentCard.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
            paymentCard.CardNumber = string.IsNullOrWhiteSpace(paymentRequest.CardNumber) ? paymentCard.CardNumber : paymentRequest.CardNumber;
            paymentCard.CustomerAddress = string.IsNullOrWhiteSpace(paymentRequest.CustomerAddress) ? paymentCard.CustomerAddress : paymentRequest.CustomerAddress;
            paymentCard.CustomerName = string.IsNullOrWhiteSpace(paymentRequest.CustomerName) ? paymentCard.CustomerName : paymentRequest.CustomerName;

            customer.CustomerName = paymentRequest.CustomerName == default(string) ? customer.CustomerName : paymentRequest.CustomerName;
            customer.CustomerAddress = paymentRequest.CustomerAddress == default(string) ? customer.CustomerAddress : paymentRequest.CustomerAddress;

            if (paymentRequest.CustomerReference == Guid.Empty)
            {
                var custRef = Guid.NewGuid();
                customer.CustomerReference = custRef;
                paymentCard.CustomerReference = custRef;

                await _customerRepository.Insert(customer);
                await _paymentCardRepository.Insert(paymentCard);
            }
            else
            {
                await _customerRepository.Update(customer);
                await _paymentCardRepository.Update(paymentCard);
            }

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
