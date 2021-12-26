using Cko.PaymentGateway.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cko.PaymentGateway.Repository
{
    public class CustomerRepository : DapperRepository<Customer>
    {
        public CustomerRepository():this(null,null)
        {

        }

        public CustomerRepository(IConfiguration settings, ILogger<CustomerRepository> logger) : base(settings, logger)
        {
        }

        public async Task<Customer> GetCustomerByReference(Guid customerReference)
        {
            var r = await Get("select top 1 * from Customer where CustomerReference = @customerReference", new { CustomerReference = customerReference });
            return r.FirstOrDefault();
        }


    }
}
