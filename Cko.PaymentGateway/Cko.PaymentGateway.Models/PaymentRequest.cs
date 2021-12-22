using Cko.PaymentGateway.Entities;

namespace Cko.PaymentGateway.Models
{
    public class PaymentRequest
    {
        public int MerchantId { get; set; }
        public Payment Payment { get; set; }
        public Guid PaymentReference { get; set; }
    }
}