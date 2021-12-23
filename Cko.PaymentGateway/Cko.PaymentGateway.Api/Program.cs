using Cko.PaymentGateway.Repository;
using Cko.PaymentGateway.Services;
using Serilog;
using Refit;
using Cko.PaymentGateway.Models;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();
    builder.Services.AddScoped<PaymentRepository>();

    builder.Services.AddRefitClient<IBankSdk>();

    // Add Serilog
    builder.Host.UseSerilog((ctx, lc) =>
        lc.WriteTo.Console()
          .ReadFrom.Configuration(ctx.Configuration)
    );

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    app.UseSerilogRequestLogging();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

