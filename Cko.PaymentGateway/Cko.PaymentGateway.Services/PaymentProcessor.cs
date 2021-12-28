using AutoMapper;
using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Logging;
using Refit;

namespace Cko.PaymentGateway.Services
{

    public interface IPaymentProcessor
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
        Task<Payment> GetPaymentDetails(string paymentReference);
    }

    /// <summary>
    /// Payment processor service
    /// </summary>
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly PaymentRepository _paymentRepository;
        private readonly BankRepository _bankRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly PaymentCardRepository _paymentCardRepository;
        private readonly MerchantRepository _merchantRepository;
        private readonly IMapper _mapper;

        public PaymentProcessor(ILogger<PaymentProcessor> logger,
                                PaymentRepository paymentRepository,
                                BankRepository bankRepository,
                                CustomerRepository customerRepository,
                                PaymentCardRepository paymentCardRepository,
                                MerchantRepository merchantRepository,
                                IMapper mapper)
        {
            this._logger = logger;
            this._paymentRepository = paymentRepository;
            this._bankRepository = bankRepository;
            this._customerRepository = customerRepository;
            this._paymentCardRepository = paymentCardRepository;
            this._merchantRepository = merchantRepository;
            this._mapper = mapper;
        }

        public async Task<Payment> GetPaymentDetails(string paymentReference)
        {
            var payment = await _paymentRepository.GetPaymentDetails(paymentReference);
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

            PaymentResponse? paymentResponse = new PaymentResponse();

            // create a new Payment object
            var payment = _mapper.Map<Payment>(paymentRequest);
            var paymentId = await _paymentRepository.Insert(payment);

            _logger.LogDebug("Created a new Payment with id={paymentId}", paymentId);

            // Create a payment entity from the request, we will then validate using the Entity
            var (valid, validationMessage) = IsValid(paymentRequest);
            payment.State = PaymentState.Validated;

            if (valid)
            {
                var merchant = await _merchantRepository.GetMerchantByIdentifier(paymentRequest.MerchantId);
                if (merchant == null)
                {
                    paymentResponse.Status = PaymentResponseStatus.Rejected_MerchantNotFound;
                    paymentResponse.PaymentResponseMessage = $"The supplied Merchant with Id = {paymentRequest.MerchantId} not found";

                    await UpdatePaymentState(payment,PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                    return paymentResponse;
                }

                Customer customer = new Customer();
                PaymentCard paymentCard = new PaymentCard();

                if (paymentRequest.CustomerReference != Guid.Empty)
                {
                    customer = await _customerRepository.GetCustomerByReference(paymentCard.CustomerReference);
                    paymentCard = await _paymentCardRepository.GetPaymentCardByCustomerReference(paymentCard.CustomerReference);

                    if (customer == null)
                    {
                        paymentResponse.Status = PaymentResponseStatus.Bank_NotFound;
                        paymentResponse.PaymentResponseMessage = $"The supplied Customer with reference = {paymentRequest.CustomerReference} not found";

                        await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);

                        return paymentResponse;

                    }
                }

                paymentCard.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
                paymentCard.CardNumber = paymentRequest.CardNumber == default(string) ? paymentCard.CardNumber : paymentRequest.CardNumber;
                paymentCard.CustomerAddress = paymentRequest.CustomerAddress == default(string) ? paymentCard.CustomerAddress : paymentRequest.CustomerAddress;
                paymentCard.CustomerName = paymentRequest.CustomerName == default(string) ? paymentCard.CustomerName : paymentRequest.CustomerName;

                customer.CustomerName = paymentRequest.CustomerName == default(string) ? customer.CustomerName : paymentRequest.CustomerName;
                customer.CustomerAddress = paymentRequest.CustomerAddress == default(string) ? customer.CustomerAddress : paymentRequest.CustomerAddress;

                if (paymentRequest.SaveCustomerDetails)
                {
                    if (paymentRequest.CustomerReference == Guid.Empty)
                    {
                        await _customerRepository.Insert(customer);
                        await _paymentCardRepository.Insert(paymentCard);
                    }
                    else
                    {
                        await _customerRepository.Update(customer);
                        await _paymentCardRepository.Update(paymentCard);
                    }
                }

                paymentResponse.Status = PaymentResponseStatus.Validated;
                paymentResponse.PaymentResponseMessage = "Card accepted, sending to bank";

                var bankId = paymentRequest.BankIdentifierCode;
                var bank = await _bankRepository.GetBankByIdentifier(bankId);

                if (bank == null)
                {
                    paymentResponse.Status = PaymentResponseStatus.Bank_NotFound;
                    paymentResponse.PaymentResponseMessage = $"The supplied Bank with code = {paymentRequest.BankIdentifierCode} not found";

                    await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);

                    return paymentResponse;
                }

                var bankPaymentReq = MakeBankPaymentRequest(paymentCard);

                var banksdk = RestService.For<IBankSdk>(bank.BankApiUrl);

                var bankResponse = await banksdk.ProcessPayment(bankPaymentReq);
                if (bankResponse.BankReponseCode == 0)
                {
                    paymentResponse.Status = PaymentResponseStatus.Approved;
                    paymentResponse.PaymentResponseMessage = "Payment processed";

                    await UpdatePaymentState(payment, PaymentState.Approved, paymentResponse.PaymentResponseMessage);
                }
                else
                {
                    paymentResponse.Status = PaymentResponseStatus.Rejected_DeclinedByBank;
                    paymentResponse.PaymentResponseMessage = bankResponse.Message;

                    await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                }
            }
            else
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_CardValidationFailed;
                paymentResponse.PaymentResponseMessage = validationMessage;

                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
            }

            return paymentResponse;
        }

        private BankPaymentRequest MakeBankPaymentRequest(PaymentCard paymentCard)
        {
            var bankReq = new BankPaymentRequest();
            bankReq.CustomerName = paymentCard.CustomerName;
            bankReq.CardNumber = paymentCard.CardNumber;
            bankReq.CustomerAddress = paymentCard.CustomerAddress;
            bankReq.Cvv = paymentCard.Cvv;

            return bankReq;
        }

        internal (bool result, string message) IsValid(PaymentRequest payment)
        {
            var validator = new PaymentRequestValidator();
            var results = validator.Validate(payment);
            if (!results.IsValid)
            {

                return (false, results.ToString(Environment.NewLine));
            }

            return (true, string.Empty);
        }
    }
}