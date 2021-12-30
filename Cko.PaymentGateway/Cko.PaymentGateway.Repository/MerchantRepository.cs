using Cko.PaymentGateway.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cko.PaymentGateway.Repository
{
    public class MerchantRepository : DapperRepository<Merchant>, IMerchantRepository
    {
        public MerchantRepository() : this(null, null)
        {

        }
        public MerchantRepository(IConfiguration settings, ILogger<MerchantRepository> logger) : base(settings, logger)
        {
        }

        public async Task<Merchant> GetMerchantByIdentifier(int merchantId)
        {
            var r = await Get("select top 1 * from Merchant where MerchantId = @MerchantId", new { MerchantId = merchantId });
            return r.FirstOrDefault();
        }


    }
}
