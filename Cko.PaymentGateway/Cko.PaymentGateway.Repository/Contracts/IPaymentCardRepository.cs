using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Repository
{
    public interface IPaymentCardRepository : IRepository<PaymentCard>
    {
        Task<PaymentCard> GetPaymentCardByCustomerReference(Guid customerReference);
    }
}