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
        public string CardNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Cvv { get; set; }
    }
}
