using Dapper.Contrib.Extensions;

namespace Cko.PaymentGateway.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public string PaymentReference { get; set; }
        public int MerchantId { get; set; }
        public DateTime PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string Ccy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }

        public string ItemDetails { get; set; }
    }


}