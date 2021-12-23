using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cko.PaymentGateway.Services
{

    public interface IPaymentProcessor
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
        Task<Payment> GetPaymentDetails(string paymentReference);
    }

    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger<PaymentProcessor> _logger;
        private readonly PaymentRepository _paymentRepository;
        private readonly IBankSdk _bank;

        public PaymentProcessor(ILogger<PaymentProcessor> logger, PaymentRepository paymentRepository, IBankSdk bank)
        {
            this._logger = logger;
            this._paymentRepository = paymentRepository;
            this._bank = bank;
        }

        public async Task<Payment> GetPaymentDetails(string paymentReference)
        {
            var payment = await _paymentRepository.GetPaymentDetails(paymentReference);
            return payment;
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
        {
            PaymentResponse? paymentResponse = null;
            if (IsValid(paymentRequest.Payment))
            {
                var bankPaymentRequest = new BankPaymentRequest();

                var bankResponse = await _bank.ProcessPayment(bankPaymentRequest);
                if (bankResponse.BankReponseCode == 0)
                {
                    paymentResponse.Status = PaymentResponseStatus.Approved;
                }    
            }
            else
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected;
            }
           
            return paymentResponse;
        }

        private bool IsValid(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}