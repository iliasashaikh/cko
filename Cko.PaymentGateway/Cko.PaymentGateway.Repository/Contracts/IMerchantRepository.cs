using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Repository
{
    public interface IMerchantRepository : IRepository<Merchant>
    {
        Task<Merchant> GetMerchantByIdentifier(int merchantId);
    }
}