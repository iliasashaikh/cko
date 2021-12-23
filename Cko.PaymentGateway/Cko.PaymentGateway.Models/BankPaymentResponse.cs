using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{
    public class BankPaymentResponse
    {
        public int BankReponseCode { get; set; }
        public Guid PaymentReference { get; set; }
    }
}
