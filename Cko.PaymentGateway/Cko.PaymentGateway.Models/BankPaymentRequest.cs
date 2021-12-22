using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{
    internal class BankPaymentRequest
    {
        public int BankId { get; set; }
        public int CardNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string MyProperty { get; set; }
    }
}
