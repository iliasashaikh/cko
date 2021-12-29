using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SaveCustomerDetails(PaymentRequest paymentRequest)
        {

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
