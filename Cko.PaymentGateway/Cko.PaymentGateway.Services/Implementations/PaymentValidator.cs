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
    public class PaymentValidator : IPaymentValidator
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentCardRepository _paymentCardRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentPersistor _paymentPersistor;

        public PaymentValidator(ICustomerRepository customerRepository,
                                IPaymentCardRepository paymentCardRepository,
                                IMerchantRepository merchantRepository,
                                IPaymentPersistor paymentPersistor)
        {
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._paymentPersistor = paymentPersistor;
        }

        public async Task<(bool, PaymentResponse)> IsValidMerchant(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            var merchant = await _merchantRepository.GetMerchantByIdentifier(paymentRequest.MerchantId);
            if (merchant == null)
            {
                paymentResponse.PaymentResponseMessage += $"The supplied Merchant with Id = {paymentRequest.MerchantId} not found.{Environment.NewLine}";
                return (false, paymentResponse);
            }

            return (true, paymentResponse);
        }

        public async Task<(bool, PaymentResponse, Customer, PaymentCard)> IsValidCustomer(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            bool valid = true;
            Customer customer = null;
            PaymentCard paymentCard = null;

            // if customer ref has been supplied it should exist in the system
            if (paymentRequest.CustomerReference != Guid.Empty)
            {
                customer = await _customerRepository.GetCustomerByReference(paymentRequest.CustomerReference);
                paymentCard = await _paymentCardRepository.GetPaymentCardByCustomerReference(paymentRequest.CustomerReference);
                valid = customer != null && paymentCard != null;

                if (!valid)
                    paymentResponse.PaymentResponseMessage += $"The supplied Customer/PaymentCard with reference = {paymentRequest.CustomerReference} not found.{Environment.NewLine}";

            }

            return (valid, paymentResponse, customer, paymentCard);
        }

        public (bool, PaymentResponse) IsValidPayment(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            var validator = new PaymentRequestValidator();
            var results = validator.Validate(paymentRequest);

            if (!results.IsValid)
            {
                paymentResponse.PaymentResponseMessage += results.ToString(Environment.NewLine);
                return (false, paymentResponse);
            }

            return (true, paymentResponse);
        }
    }
}
