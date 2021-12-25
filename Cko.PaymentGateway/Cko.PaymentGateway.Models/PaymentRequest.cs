using Cko.PaymentGateway.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cko.PaymentGateway.Models
{
    public class PaymentRequest
    {

        // Customer details
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public Guid CustomerReference { get; set; }

        // Card details
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public DateTime CardExpiry { get; set; }
        public string BankIdentifierCode { get; set; }

        // Purchase Info
        public string? ItemDetails { get; set; }
        public decimal Amount { get; set; }
        public string? Ccy { get; set; }


        // Merchant Details
        public int MerchantId { get; set; }
        public int MerchantName { get; set; }

    }
}