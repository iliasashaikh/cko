using AutoMapper;
using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Logging;
using Refit;

namespace Cko.PaymentGateway.Services
{

    public interface IPaymentProcessor2
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
        Task<Payment> GetPaymentDetails(int paymentId);
    }

    /// <summary>
    /// Payment processor service. Processes, stores and retrieves payments 
    /// </summary>
    public class PaymentProcessor2 : IPaymentProcessor2
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

        public PaymentProcessor2(ILogger<PaymentProcessor> logger,
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

            PaymentResponse? paymentResponse = new PaymentResponse { PaymentId = paymentId };

            (var valid, paymentResponse) = _paymentValidator.IsValid(paymentRequest, paymentResponse);

            if (!valid)
            {
                return paymentResponse;
            }

            await _paymentPersistor.SaveCustomerDetails(paymentRequest);
            await _bankInteface.Pay(paymentCard);
        }

            

            // create a new Payment object
            var payment = _mapper.Map<Payment>(paymentRequest);
            payment.State = PaymentState.Validated;
            var paymentId = await _paymentRepository.Insert(payment);

            _logger.LogDebug("Created a new Payment with id={paymentId}", paymentId);


            var merchant = await _merchantRepository.GetMerchantByIdentifier(paymentRequest.MerchantId);
            if (merchant == null)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_MerchantNotFound;
                paymentResponse.PaymentResponseMessage = $"The supplied Merchant with Id = {paymentRequest.MerchantId} not found";

                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                return paymentResponse;
            }

            Customer customer = new Customer();
            PaymentCard paymentCard = new PaymentCard();



            // If the merchant passes in a Customer reference, then it should exist in the system
            if (paymentRequest.CustomerReference != Guid.Empty)
            {
                customer = await _customerRepository.GetCustomerByReference(paymentRequest.CustomerReference);
                paymentCard = await _paymentCardRepository.GetPaymentCardByCustomerReference(paymentRequest.CustomerReference);

                if (customer == null)
                {
                    paymentResponse.Status = PaymentResponseStatus.Rejected_BankNotFound;
                    paymentResponse.PaymentResponseMessage = $"The supplied Customer with reference = {paymentRequest.CustomerReference} not found";

                    await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);

                    return paymentResponse;

                }

                // Fill in payment details from the database if needed
                paymentRequest.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
                paymentRequest.CustomerName = string.IsNullOrWhiteSpace(paymentRequest.CustomerName) ? paymentCard.CustomerName : paymentRequest.CustomerName;
                paymentRequest.CustomerAddress = string.IsNullOrWhiteSpace(paymentRequest.CustomerAddress) ? paymentCard.CustomerName : paymentRequest.CustomerName;
                paymentRequest.CardNumber = string.IsNullOrWhiteSpace(paymentRequest.CardNumber) ? paymentCard.CardNumber : paymentRequest.CardNumber;
                paymentRequest.Cvv = string.IsNullOrWhiteSpace(paymentRequest.Cvv) ? paymentCard.Cvv : paymentRequest.Cvv;
                paymentRequest.BankIdentifierCode = string.IsNullOrWhiteSpace(paymentRequest.BankIdentifierCode) ? paymentCard.BankIdentifierCode : paymentRequest.BankIdentifierCode;

            }

            var (valid, validationMessage) = IsValid(paymentRequest);

            if (!valid)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_CardValidationFailed;
                paymentResponse.PaymentResponseMessage = validationMessage;

                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);
                return paymentResponse;
            }

            // fill in customer info from the payment Request
            paymentCard.CardExpiry = paymentRequest.CardExpiry == default(DateTime) ? paymentCard.CardExpiry : paymentRequest.CardExpiry;
            paymentCard.CardNumber = string.IsNullOrWhiteSpace(paymentRequest.CardNumber) ? paymentCard.CardNumber : paymentRequest.CardNumber;
            paymentCard.CustomerAddress = string.IsNullOrWhiteSpace(paymentRequest.CustomerAddress) ? paymentCard.CustomerAddress : paymentRequest.CustomerAddress;
            paymentCard.CustomerName = string.IsNullOrWhiteSpace(paymentRequest.CustomerName) ? paymentCard.CustomerName : paymentRequest.CustomerName;

            customer.CustomerName = paymentRequest.CustomerName == default(string) ? customer.CustomerName : paymentRequest.CustomerName;
            customer.CustomerAddress = paymentRequest.CustomerAddress == default(string) ? customer.CustomerAddress : paymentRequest.CustomerAddress;

            // Save if needed
            if (paymentRequest.SaveCustomerDetails)
            {
                if (paymentRequest.CustomerReference == Guid.Empty)
                {
                    var custRef = Guid.NewGuid();
                    customer.CustomerReference = custRef;
                    payment.CustomerReference = custRef;
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

            paymentResponse.Status = PaymentResponseStatus.Validated;
            paymentResponse.PaymentResponseMessage = "Card accepted, sending to bank";

            var bankId = paymentRequest.BankIdentifierCode;
            var bank = await _bankRepository.GetBankByIdentifier(bankId);

            if (bank == null)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_BankNotFound;
                paymentResponse.PaymentResponseMessage = $"The supplied Bank with code = {paymentRequest.BankIdentifierCode} not found";

                await UpdatePaymentState(payment, PaymentState.Rejected, paymentResponse.PaymentResponseMessage);

                return paymentResponse;
            }

            var bankPaymentReq = MakeBankPaymentRequest(paymentCard);

            IBankSdk banksdk = null;
            try
            {
                banksdk = _bankFunc(bank.BankApiUrl);
            }
            catch (Exception)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_UnableToConnectToBank;
                paymentResponse.PaymentResponseMessage = "Unable to connect to the bank";

                await UpdatePaymentState(payment, PaymentState.UnableToConnectToBank, paymentResponse.PaymentResponseMessage);

                throw;
            }

            var bankResponse = await banksdk.ProcessPayment(bankPaymentReq);
            if (bankResponse.BankReponseCode == 0)
            {
                paymentResponse.Status = PaymentResponseStatus.Approved;
                paymentResponse.PaymentResponseMessage = "Payment processed";
                paymentResponse.PaymentReference = bankResponse.PaymentReference;
                payment.PaymentReference = bankResponse.PaymentReference;

                await UpdatePaymentState(payment, PaymentState.Approved, paymentResponse.PaymentResponseMessage);
            }
            else
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_DeclinedByBank;
                paymentResponse.PaymentResponseMessage = bankResponse.BankPaymentResponseMessage;

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
            bankReq.CardExpiry = paymentCard.CardExpiry;

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