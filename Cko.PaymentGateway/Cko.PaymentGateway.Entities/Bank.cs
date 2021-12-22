using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Entities
{
    public class Bank
    {
        [Key]
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string BankIdentifier { get; set; }
    }
}
