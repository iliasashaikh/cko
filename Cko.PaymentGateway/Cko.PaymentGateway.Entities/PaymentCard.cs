using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Entities
{
    public class PaymentCard
    {
        [Key]
        public int PaymentCardId { get; set; }
        public int BankId { get; set; }
        public int MerchantId { get; set; }
        public DateTime CardExpiry { get; set; }
        public string CardNumber { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
    }
}
