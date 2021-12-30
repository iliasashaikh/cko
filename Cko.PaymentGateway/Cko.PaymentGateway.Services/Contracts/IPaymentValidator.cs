using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;

namespace Cko.PaymentGateway.Services
{
    public interface IPaymentValidator
    {
        Task<(bool, PaymentResponse, Customer, PaymentCard)> IsValidCustomer(PaymentRequest paymentRequest, PaymentResponse paymentResponse);
        Task<(bool, PaymentResponse)> IsValidMerchant(PaymentRequest paymentRequest, PaymentResponse paymentResponse);
        (bool, PaymentResponse) IsValidPayment(PaymentRequest paymentRequest, PaymentResponse paymentResponse);
    }
}