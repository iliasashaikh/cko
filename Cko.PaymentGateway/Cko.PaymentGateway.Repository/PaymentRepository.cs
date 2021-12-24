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
        public PaymentRepository(IConfiguration settings, ILogger<PaymentRepository> logger) : base(settings, logger)
        {
        }

        public async Task<Payment> GetPaymentDetails(string paymentReference)
        {
            return await Task.Run(() => new Payment());
        }
    }


}
