using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Repository
{
    public interface IBankRepository : IRepository<Bank> 
    {
        Task<Bank> GetBankByIdentifier(string bankId);
    }
}