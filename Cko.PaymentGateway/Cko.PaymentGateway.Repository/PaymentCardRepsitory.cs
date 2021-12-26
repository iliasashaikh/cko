using Cko.PaymentGateway.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Repository
{
    public class PaymentCardRepository : DapperRepository<PaymentCard>
    {
        public PaymentCardRepository() : this(null,null)
        {

        }

        public PaymentCardRepository(IConfiguration settings, ILogger<PaymentCardRepository> logger) : base(settings, logger)
        {
        }

        public async Task<PaymentCard> GetPaymentCardByCustomerReference(Guid customerReference)
        {
            var r = await Get("select top 1 * from PaymentCard where CustomerReference = @customerReference", new { CustomerReference = customerReference });
            return r.FirstOrDefault();
        }


    }
}
