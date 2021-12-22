﻿using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cko.PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessPaymentsController : ControllerBase
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public ProcessPaymentsController(IPaymentProcessor paymentProcessor)
        {
            this._paymentProcessor = paymentProcessor;
        }

        [HttpPost(Name = "ProcessPayment")]
        public IActionResult ProcessPayment(PaymentRequest paymentRequest)
        {
            var response = _paymentProcessor.ProcessPayment(paymentRequest);
            return Ok(response);
        }
    }
}