using MyBank.Api;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("");
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.MapPost("/pay",(BankPaymentRequest req)=>
{
    // check hard coded cases

    if (req.CardExpiry < DateTime.Today)
        return Results.BadRequest("Card Expired");

    if (req.CardNumber == "4111111111111111")
        return Results.BadRequest("Card blocked. Please contact your Bank to unblock");

    var resp = new BankPaymentResponse { BankPaymentResponseMessage = "Payment Successful", BankReponseCode = 0, PaymentReference = Guid.NewGuid()};
    return Results.Ok();
}
);


app.Run();