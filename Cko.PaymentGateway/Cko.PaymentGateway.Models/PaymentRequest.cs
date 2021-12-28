using Cko.PaymentGateway.Entities;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Cko.PaymentGateway.Models
{
    public class PaymentRequest
    {

        // Customer details
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public Guid CustomerReference { get; set; }

        // Card details
        public string CardNumber { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public DateTime CardExpiry { get; set; } = DateTime.UtcNow.AddDays(-1);
        public string BankIdentifierCode { get; set; } = string.Empty;
        public bool SaveCustomerDetails { get; set; }

        // Purchase Info
        public string ItemDetails { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Ccy { get; set; } = string.Empty;
        public DateTime PaymentTime { get; set; } = DateTime.UtcNow;


        // Merchant Details
        public int MerchantId { get; set; }
        public string MerchantName { get; set; } = string.Empty;


    }

    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(p => p.Amount).NotEmpty().GreaterThan(0);
            RuleFor(p => p.Ccy).NotEmpty().Length(3);

            RuleFor(p => p.MerchantId).NotEmpty();

            RuleFor(p => p.CustomerReference).NotEmpty()
                                             .When
                                                (p =>
                                                    string.IsNullOrEmpty(p.CustomerName) ||
                                                    string.IsNullOrEmpty(p.CustomerAddress) ||
                                                    string.IsNullOrEmpty(p.CardNumber) ||
                                                    string.IsNullOrEmpty(p.Cvv) ||
                                                    string.IsNullOrEmpty(p.BankIdentifierCode)
                                                    )
                                                .WithMessage("All Customer and Card details should be filled in when Cusotmer reference is not provided");

            RuleFor(p => p.CustomerAddress).NotEmpty().When(p => p.CustomerReference == default(Guid));
            RuleFor(p => p.CustomerName).NotEmpty().When(p => p.CustomerReference == default(Guid));
            RuleFor(p => p.CustomerAddress).NotEmpty().When(p => p.CustomerReference == default(Guid));
            RuleFor(p => p.CardNumber).NotEmpty().CreditCard().When(p => p.CustomerReference == default(Guid));
            RuleFor(p => p.Cvv).NotEmpty().When(p => p.CustomerReference == default(Guid));
            RuleFor(p => p.BankIdentifierCode).NotEmpty().When(p => p.CustomerReference == default(Guid));
        }
    }

}