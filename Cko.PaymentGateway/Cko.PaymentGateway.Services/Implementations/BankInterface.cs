using Cko.PaymentGateway.Entities;
using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Services
{
    public class BankInterface : IBankInterface
    {
        private readonly ILogger<BankInterface> _logger;
        private readonly IBankRepository _bankRepository;
        private readonly Func<string, IBankSdk> _bankFunc;

        public BankInterface(ILogger<BankInterface> logger,
                                IBankRepository bankRepository,
                                Func<string, IBankSdk> bankFunc)
        {
            this._logger = logger;
            this._bankRepository = bankRepository;
            this._bankFunc = bankFunc;
        }

        public async Task<(bool, PaymentResponse, BankPaymentResponse?)> Pay(PaymentRequest paymentRequest, PaymentResponse paymentResponse, PaymentCard paymentCard)
        {
            var bankId = paymentRequest.BankIdentifierCode;
            var bank = await _bankRepository.GetBankByIdentifier(bankId);

            if (bank == null)
            {
                paymentResponse.Status = PaymentResponseStatus.Rejected_UnableToConnectToBank;
                paymentResponse.PaymentResponseMessage = $"The supplied Bank with code = {paymentRequest.BankIdentifierCode} not found";

                return (false, paymentResponse, null);
            }

            var bankPaymentReq = MakeBankPaymentRequest(paymentCard);

            IBankSdk? banksdk = null;
            try
            {
                banksdk = _bankFunc(bank.BankApiUrl);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message, error);
                paymentResponse.PaymentResponseMessage = "Unable to connect to the bank";

                return (false, paymentResponse, null);
            }

            var bankResponse = await banksdk.ProcessPayment(bankPaymentReq);
            if (bankResponse.BankReponseCode == 0) // success!
            {
                paymentResponse.PaymentResponseMessage = "Payment processed";
                paymentResponse.PaymentReference = bankResponse.PaymentReference;

                return (true, paymentResponse, bankResponse);
            }
            else
            {
                paymentResponse.PaymentResponseMessage = bankResponse.BankPaymentResponseMessage;
                return (false, paymentResponse, bankResponse);
            }
        }

        private BankPaymentRequest MakeBankPaymentRequest(PaymentCard paymentCard)
        {
            var bankReq = new BankPaymentRequest();
            bankReq.CustomerName = paymentCard.CustomerName;
            bankReq.CardNumber = paymentCard.CardNumber;
            bankReq.CustomerAddress = paymentCard.CustomerAddress;
            bankReq.Cvv = paymentCard.Cvv;
            bankReq.CardExpiry = paymentCard.CardExpiry;

            return bankReq;
        }
    }
}
