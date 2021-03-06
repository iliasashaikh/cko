using Cko.PaymentGateway.Models;
using Cko.PaymentGateway.Repository;
using Cko.PaymentGateway.Services;
using Microsoft.OpenApi.Models;
using Refit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Payment Gateway starting up....");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Pay faster.....with Checkout.com",
            Description = "Checkout API Built with .NET Core 6",
        });
    });

    builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

    Func<string, IBankSdk> f = (url) => RestService.For<IBankSdk>(url);
    builder.Services.AddScoped<Func<string, IBankSdk>>((s) => f);
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.Scan(scan => scan.FromAssemblyOf<PaymentRepository>()
                                        .AddClasses()
                                        .AsSelf()
                                        .AsImplementedInterfaces()
                                        .WithTransientLifetime());

    builder.Services.Scan(scan => scan.FromAssemblyOf<PaymentProcessor>()
                                        .AddClasses()
                                        .AsSelf()
                                        .AsImplementedInterfaces()
                                        .WithTransientLifetime());

    // Add Serilog
    builder.Host.UseSerilog((ctx, lc) =>
        lc.WriteTo.Console()
          .ReadFrom.Configuration(ctx.Configuration)
    );

    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });


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

public partial class Program { }