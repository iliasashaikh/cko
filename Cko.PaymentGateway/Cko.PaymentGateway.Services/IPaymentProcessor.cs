using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;

namespace Cko.PaymentGateway.Services
{
    public interface IPaymentProcessor
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
        Task<Payment> GetPaymentDetails(int paymentId);
    }
}