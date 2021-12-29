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
    public class PaymentValidator
    {
        private readonly CustomerRepository _customerRepository;
        private readonly PaymentCardRepository _paymentCardRepository;
        private readonly MerchantRepository _merchantRepository;
        private readonly PaymentPersistor _paymentPersistor;

        public PaymentValidator(CustomerRepository customerRepository,
                                PaymentCardRepository paymentCardRepository,
                                MerchantRepository merchantRepository,
                                PaymentPersistor paymentPersistor)
        {
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._paymentPersistor = paymentPersistor;
        }

        internal async Task<(bool, PaymentResponse)> IsValidMerchant(Payment payment, PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            var merchant = await _merchantRepository.GetMerchantByIdentifier(paymentRequest.MerchantId);
            if (merchant == null)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_MerchantNotFound;
                paymentResponse.PaymentResponseMessage = $"The supplied Merchant with Id = {paymentRequest.MerchantId} not found";

                await _paymentPersistor.UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                return (false,paymentResponse);
            }

            return (false, paymentResponse);
        }

        internal (bool, PaymentResponse) IsValidCustomer(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            
        }
        internal (bool, PaymentResponse) IsValidPayment(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            throw new NotImplementedException();
        }

        public (bool, PaymentResponse) IsValid(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            
            (var isValidMerchant, paymentResponse) = IsValidMerchant(paymentRequest, paymentResponse);
            (var isValidCustomer, paymentResponse) = IsValidCustomer(paymentRequest, paymentResponse);
            (var isValidPayment, paymentResponse) = IsValidCustomer(paymentRequest, paymentResponse);

            return (isValidCustomer && isValidPayment && isValidMerchant, PaymentResponse);
        }

    }
}
