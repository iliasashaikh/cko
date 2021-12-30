using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Repository
{
    public interface IRepository<T>
        where T : class
    {
        public Task<IEnumerable<T>> Get(string query, object? parameters = null);
        public Task Run(string query, object? parameters = null);
        public Task<int> Insert(T entity);
        public Task<bool> Update(T entity);
    }
}
