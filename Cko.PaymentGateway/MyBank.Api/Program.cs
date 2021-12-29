using MyBank.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.MapPost("/pay",(BankPaymentRequest req)=>
{
    var resp = new BankPaymentResponse { BankPaymentResponseMessage = "Payment Successful", BankReponseCode = 0, PaymentReference = Guid.NewGuid()};

    // check sample hard coded cases
    if (req.CardExpiry < DateTime.Today)
    {
        resp.BankReponseCode = -1;
        resp.BankPaymentResponseMessage = "Card expired.";
    }
    else
    if (req.CardNumber == "4111111111111111")
    {
        resp.BankReponseCode = -2;
        resp.BankPaymentResponseMessage = "Card blocked. Please contact your Bank to unblock";
    }

    return Results.Ok(resp);
}
);


app.Run();