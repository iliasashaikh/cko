using AutoMapper;
using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Logging;

namespace Cko.PaymentGateway.Services
{

    /// <summary>
    /// Payment processor service. Processes, stores and retrieves payments
    /// </summary>
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly IPaymentRepository _paymentRepository;

        private readonly IPaymentValidator _paymentValidator;
        private readonly IPaymentPersistor _paymentPersistor;
        private readonly IBankInterface _bankInterface;
        private readonly IMapper _mapper;

        public PaymentProcessor(ILogger<PaymentProcessor> logger,
                                IPaymentValidator paymentValidator,
                                IPaymentPersistor paymentPersistor,
                                IBankInterface bankInterface,
                                IPaymentRepository paymentRepository,
                                IMapper mapper)
        {
            this._logger = logger;
            this._paymentRepository = paymentRepository;
            this._paymentValidator = paymentValidator;
            this._paymentPersistor = paymentPersistor;
            this._bankInterface = bankInterface;
            this._mapper = mapper;
        }

        public async Task<Payment> GetPaymentDetails(int paymentId)
        {
            var payment = await _paymentRepository.GetPaymentDetails(paymentId);
            return payment;
        }

        private async Task UpdatePaymentState(Payment payment, PaymentState state, string info)
        {
            (payment.State, payment.PaymentInfo) = (state, info);
            _ = await _paymentRepository.Update(payment);
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
        {
            _logger.LogDebug("Received {@paymentRequest}", paymentRequest);
            var payment = _mapper.Map<Payment>(paymentRequest);

            await _paymentPersistor.SavePayment(payment);
            var paymentId = payment.PaymentId;
            _logger.LogDebug("Created a new Payment with id={paymentId}", paymentId);

            PaymentResponse? paymentResponse = new PaymentResponse { PaymentId = paymentId };

            // Run all the validations for payment
            (var validMerchant, paymentResponse) = await _paymentValidator.IsValidMerchant(paymentRequest, paymentResponse);
            (var validCustomer, paymentResponse, var customer, var paymentCard) = await _paymentValidator.IsValidCustomer(paymentRequest, paymentResponse);

            _logger.LogDebug("Payment validated");

            // Fill in payment details from the database if needed
            if (paymentCard != null)
                EnrichPaymentRequest(paymentCard, paymentRequest);

            (var validPaymentDetails, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            bool valid = validCustomer && validMerchant && validPaymentDetails;
            if (!valid)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_ValidationFailed;
                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                return paymentResponse;
            }

            // If the request is valid and we need to enrich and possibly save Cust & Card details
            (customer, paymentCard) = await _paymentPersistor.SaveCustomerDetails(paymentRequest, customer ?? new Customer(), paymentCard ?? new PaymentCard());

            // set the payment card and customer references
            payment.CustomerReference = customer.CustomerReference;
            payment.PaymentCardId = paymentCard.PaymentCardId;

            (var paid, paymentResponse, var bankPaymentResponse) = await _bankInterface.Pay(paymentRequest, paymentResponse, paymentCard);
            if (!paid)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_PaymentFailed;
                await UpdatePaymentState(payment, PaymentState.PaymentFailed, paymentResponse.PaymentResponseMessage);
                return paymentResponse;
            }

            // Payment succeeded!
            payment.PaymentReference = bankPaymentResponse.PaymentReference;
            paymentResponse.Status = PaymentResponseStatus.Approved;
            await UpdatePaymentState(payment, PaymentState.Approved, paymentResponse.PaymentResponseMessage);
            _logger.LogDebug("Payment succeeded");

            return paymentResponse;
        }

        private void EnrichPaymentRequest(PaymentCard paymentCard, PaymentRequest paymentRequest)
        {
            if (paymentCard != null)
            {
                paymentRequest.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
                paymentRequest.CustomerName = string.IsNullOrWhiteSpace(paymentRequest.CustomerName) ? paymentCard.CustomerName : paymentRequest.CustomerName;
                paymentRequest.CustomerAddress = string.IsNullOrWhiteSpace(paymentRequest.CustomerAddress) ? paymentCard.CustomerName : paymentRequest.CustomerName;
                paymentRequest.CardNumber = string.IsNullOrWhiteSpace(paymentRequest.CardNumber) ? paymentCard.CardNumber : paymentRequest.CardNumber;
                paymentRequest.Cvv = string.IsNullOrWhiteSpace(paymentRequest.Cvv) ? paymentCard.Cvv : paymentRequest.Cvv;
                paymentRequest.BankIdentifierCode = string.IsNullOrWhiteSpace(paymentRequest.BankIdentifierCode) ? paymentCard.BankIdentifierCode : paymentRequest.BankIdentifierCode;
            }
        }

    }
}