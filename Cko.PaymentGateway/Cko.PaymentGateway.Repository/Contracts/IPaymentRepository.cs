using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Repository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetPaymentDetails(int paymentId);
    }
}