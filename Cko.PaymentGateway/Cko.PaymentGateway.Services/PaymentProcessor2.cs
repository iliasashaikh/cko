using AutoMapper;
using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Logging;
using Refit;

namespace Cko.PaymentGateway.Services
{

    /// <summary>
    /// Payment processor service. Processes, stores and retrieves payments
    /// </summary>
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly PaymentRepository _paymentRepository;
        private readonly BankRepository _bankRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly PaymentCardRepository _paymentCardRepository;
        private readonly MerchantRepository _merchantRepository;
        private readonly Func<string, IBankSdk> _bankFunc;

        private readonly PaymentValidator _paymentValidator;
        private readonly PaymentPersistor _paymentPersistor;
        private readonly BankInterface _bankInterface;
        private readonly IMapper _mapper;

        public PaymentProcessor(ILogger<PaymentProcessor> logger,
                                PaymentRepository paymentRepository,
                                BankRepository bankRepository,
                                CustomerRepository customerRepository,
                                PaymentCardRepository paymentCardRepository,
                                MerchantRepository merchantRepository,
                                Func<string, IBankSdk> bankFunc,

                                PaymentValidator paymentValidator,
                                PaymentPersistor paymentPersistor,
                                BankInterface bankInterface,
                                IMapper mapper)
        {
            this._logger = logger;
            this._paymentRepository = paymentRepository;
            this._bankRepository = bankRepository;
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._bankFunc = bankFunc;
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
            if (customer != null && paymentCard != null)
                EnrichPaymentRequest(customer, paymentCard, paymentRequest);

            (var validPaymentDetails, paymentResponse) = _paymentValidator.IsValidPayment(paymentRequest, paymentResponse);

            bool valid = validCustomer && validMerchant && validPaymentDetails;
            if (!valid)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_ValidationFailed;
                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                return paymentResponse;
            }

            // If the request is valid and we need to save Cust & Card details
            if (paymentRequest.SaveCustomerDetails)
               await _paymentPersistor.SaveCustomerDetails(paymentRequest, customer ?? new Customer(), paymentCard ?? new PaymentCard());

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

        private void EnrichPaymentRequest(Customer customer, PaymentCard paymentCard, PaymentRequest paymentRequest)
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