using Microsoft.Extensions.Configuration;
using Stripe;

namespace Rydo.Application.Interfaces.Payments.Stripe;

public class StripePaymentService
{
    private readonly IConfiguration _config;
    public StripePaymentService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<string> CreatePaymentIntent(long amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount * 100, // Stripe sử dụng cents
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.ClientSecret;
    }

    public async Task StripeWebhook()
    {
        
    }
}