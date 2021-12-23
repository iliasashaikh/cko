using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{

    public interface IBankSdk
    {
        [Post("/processpayment")]
        Task<BankPaymentResponse> ProcessPayment([Body] BankPaymentRequest paymentRequest);
    }
}
