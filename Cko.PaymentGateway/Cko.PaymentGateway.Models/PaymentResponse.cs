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
        Rejected_BankNotFound,
        Rejected_CardValidationFailed,
        Rejected_DeclinedByBank,
        Rejected_UnableToConnectToBank
    }

    public class PaymentResponse
    {
        public PaymentResponseStatus Status { get; set;} = PaymentResponseStatus.New;
        public string PaymentResponseMessage { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public Guid PaymentReference { get; set; }
    }
}
