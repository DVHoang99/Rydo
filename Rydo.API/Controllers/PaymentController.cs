using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Common.Enums;
using Rydo.Application.Orders.Commands;
using Stripe;
using Stripe.Checkout;

namespace Rydo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(request.Amount * 100),
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "wechat_pay" }, // or "alipay", "promptpay"
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return Ok(new
        {
            paymentIntentId = intent.Id,
            clientSecret = intent.ClientSecret,
            nextAction = intent.NextAction
        });
    }
    
    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];
        var secret = configuration["Stripe:WebhookSecret"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid signature", detail = ex.Message });
        }

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
            {
                var intent = (PaymentIntent)stripeEvent.Data.Object;
                var orderId = intent.Metadata.GetValueOrDefault("orderId", intent.Id);

                await mediator.Send(new UpdatePaymentStatusCommand(orderId, PaymentStatus.Successful));
                break;
            }

            case "payment_intent.payment_failed":
            {
                var intent = (PaymentIntent)stripeEvent.Data.Object;
                var orderId = intent.Metadata.GetValueOrDefault("orderId", intent.Id);

                await mediator.Send(new UpdatePaymentStatusCommand(orderId, PaymentStatus.Failed));
                break;
            }
        }

        return Ok();
    }
}