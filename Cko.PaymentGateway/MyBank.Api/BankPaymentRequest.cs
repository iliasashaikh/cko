using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBank.Api
{
    public class BankPaymentRequest
    {
        public int BankId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public DateTime CardExpiry { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
    }
}
