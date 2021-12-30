using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;

namespace Cko.PaymentGateway.Services
{
    public interface IPaymentPersistor
    {
        Task<(Customer,PaymentCard)> SaveCustomerDetails(PaymentRequest paymentRequest, Customer customer, PaymentCard paymentCard);
        Task<int> SavePayment(Payment payment);
        Task UpdatePaymentState(Payment payment, PaymentState state, string info);
    }
}