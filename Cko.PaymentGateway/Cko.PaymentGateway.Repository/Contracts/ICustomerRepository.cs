using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Repository
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByReference(Guid customerReference);
    }
}