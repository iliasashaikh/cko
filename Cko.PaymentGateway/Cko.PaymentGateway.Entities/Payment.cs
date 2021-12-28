using Dapper.Contrib.Extensions;
using FluentValidation;

namespace Cko.PaymentGateway.Entities
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public Guid CustomerReference { get; set; }
        public int MerchantId { get; set; }
        public DateTime PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string Ccy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string ItemDetails { get; set; } = string.Empty;
        public PaymentState State { get; set; }
        public string PaymentInfo { get; set; } = string.Empty;
        public int PaymentCardId { get; set; }
    }

    public enum PaymentState
    {
        UnProcessed,
        Validated,
        SentToBank,
        Approved,
        Rejected
    }
}