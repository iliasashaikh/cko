using Dapper.Contrib.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Entities
{
    public class PaymentCard
    {
        [Key]
        public int PaymentCardId { get; set; }
        public string? BankIdentifierCode { get; set; }
        public DateTime CardExpiry { get; set; }
        public string? CardNumber { get; set; }
        public Guid CustomerReference { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Cvv { get; set; }
    }

    public class PaymentCardValidator : AbstractValidator<PaymentCard>
    {
        public PaymentCardValidator()
        {
            RuleFor(c=>c.CustomerAddress).NotEmpty();
            RuleFor(c=>c.CustomerName).NotEmpty();
            RuleFor(c=>c.CardNumber).NotEmpty().CreditCard();
            RuleFor(c=>c.BankIdentifierCode).NotEmpty();
            RuleFor(c=>c.CardExpiry).NotEmpty().GreaterThan(DateTime.Today.AddDays(1));
        }
    }
}
