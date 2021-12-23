using Dapper.Contrib.Extensions;
using FluentValidation;

namespace Cko.PaymentGateway.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public string? PaymentReference { get; set; }
        public int MerchantId { get; set; }
        public DateTime PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string? Ccy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ItemDetails { get; set; }
        public PaymentCard? PaymentCard { get; set; }
    }

    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(p=>p.Amount).NotEmpty().GreaterThan(0);
            RuleFor(p => p.Ccy).NotEmpty().Length(3);
            RuleFor(p=>p.PaymentCard).NotNull().SetValidator(new PaymentCardValidator());
        }
    }

}