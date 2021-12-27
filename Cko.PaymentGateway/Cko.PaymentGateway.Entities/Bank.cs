using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Entities
{
    [Table("Bank")]
    public class Bank
    {
        [Key]
        public int BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string BankIdentifier { get; set; } = string.Empty;
        public string BankApiUrl { get; set; } = string.Empty;
    }
}
