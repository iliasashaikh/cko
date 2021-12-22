using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Entities
{
    public class Merchant
    {
        [Key]
        public int MerchantId { get; set; }
        public string Name { get; set; }
        public int Address { get; set; }

    }
}
