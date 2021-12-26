using Cko.PaymentGateway.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cko.PaymentGateway.Repository
{
    public class BankRepository : DapperRepository<Bank>
    {
        public BankRepository() : this(null,null)
        {

        }

        public BankRepository(IConfiguration settings, ILogger<BankRepository> logger) : base(settings, logger)
        {
        }

        public async Task<Bank> GetBankByIdentifier(string bankId)
        {
            var r = await Get("select top 1 * from Bank where BankIdentifier = @id", new { id = bankId });
            return r.FirstOrDefault();
        }


    }
}
