namespace MyBank.Api
{
    public class BankPaymentResponse
    {
        public int BankReponseCode { get; set; }
        public Guid PaymentReference { get; set; }
    }
}
