using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{

    public enum PaymentResponseStatus
    {
        Approved,
        Rejected
    }

    public class PaymentResponse
    {
        public Guid PaymentReference { get; set; }
        public PaymentResponseStatus Status { get; set;}
        public string Message { get; set; }
    }
}
