using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;

namespace Cko.PaymentGateway.Services
{
    public interface IBankInterface
    {
        Task<(bool, PaymentResponse, BankPaymentResponse?)> Pay(PaymentRequest paymentRequest, PaymentResponse paymentResponse, PaymentCard paymentCard);
    }
}