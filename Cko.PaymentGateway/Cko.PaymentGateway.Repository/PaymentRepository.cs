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
    public class PaymentRepository : DapperRepository<Payment>
    {
        public PaymentRepository():this(null,null)
        {

        }

        public PaymentRepository(IConfiguration settings, ILogger<PaymentRepository> logger) : base(settings, logger)
        {
        }

        public async Task<Payment> GetPaymentDetails(int paymentId)
        {
            var r = await Get(@"select * from Payment where paymentId = @PaymentId", new { PaymentId = paymentId });
            return r.FirstOrDefault();
        }
    }


}
