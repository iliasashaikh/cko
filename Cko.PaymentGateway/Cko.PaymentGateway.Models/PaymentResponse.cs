using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{

    public enum PaymentResponseStatus
    {
        New,
        Validated,
        Approved,
        Rejected_MerchantNotFound,
        Rejected_CustomerNotFound,
        Rejected_CardValidationFailed,
        Rejected_DeclinedByBank
    }

    public class PaymentResponse
    {
        public Guid PaymentReference { get; set; }
        public PaymentResponseStatus Status { get; set;} = PaymentResponseStatus.New;
        public string PaymentResponseMessage { get; set; } = string.Empty;
    }
}
