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
        private readonly IBankSdk _bankSdk;

        public PaymentProcessor(ILogger<PaymentProcessor> logger, PaymentRepository paymentRepository, BankRepository bankRepository, IBankSdk bankSdk)
        {
            this._logger = logger;
            this._paymentRepository = paymentRepository;
            this._bankRepository = bankRepository;
            this._bankSdk = bankSdk;
        }

        public async Task<Payment> GetPaymentDetails(string paymentReference)
        {
            var payment = await _paymentRepository.GetPaymentDetails(paymentReference);
            return payment;
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
        {
            PaymentResponse? paymentResponse = new PaymentResponse();
            var (valid, validationMessage) = IsValid(paymentRequest.Payment);
            if (valid)
            {
                paymentResponse.Status = PaymentResponseStatus.Validated;
                paymentResponse.Message = "Card accepted, sending to bank";

                var bankId = paymentRequest.Payment.PaymentCard.BankIdentifierCode;
                var bank = await _bankRepository.GetBankByIdentifier(bankId);
                var bankPaymentReq = MakeBankPaymentRequest(paymentRequest);
                var banksdk = RestService.For<IBankSdk>(bank.BankApiUrl);

                var bankResponse = await banksdk.ProcessPayment(bankPaymentReq);
                if (bankResponse.BankReponseCode == 0)
                {
                    paymentResponse.Status = PaymentResponseStatus.Approved;
                    paymentResponse.Message = "Payment processed";
                }
                else
                {
                    paymentResponse.Status = PaymentResponseStatus.Rejected;
                    paymentResponse.Message = bankResponse.Message;
                }
            }
            else
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected;
                paymentResponse.Message = validationMessage;
            }

            return paymentResponse;
        }

        private BankPaymentRequest MakeBankPaymentRequest(PaymentRequest paymentRequest)
        {
            var bankReq = new BankPaymentRequest();
            bankReq.CustomerName = paymentRequest.Payment.PaymentCard.CustomerName;
            bankReq.CardNumber = paymentRequest.Payment.PaymentCard.CardNumber;
            bankReq.CustomerAddress = paymentRequest.Payment.PaymentCard.CustomerAddress;
            bankReq.Cvv = paymentRequest.Payment.PaymentCard.Cvv;

            return bankReq;
        }

        private (bool result, string message) IsValid(Payment payment)
        {
            var validator = new PaymentValidator();
            var results = validator.Validate(payment);
            if (!results.IsValid)
            {

                 return (false, results.ToString("~"));
            }

            return (true, null);
        }
    }
}