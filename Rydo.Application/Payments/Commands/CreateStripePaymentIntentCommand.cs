using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Common.Interfaces;
using Stripe;

namespace Rydo.Application.Payments.Commands;

public record CreateStripePaymentIntentCommand(Guid BookingId) : IRequest<string>;

public class CreateStripePaymentIntentCommandHandler 
    : IRequestHandler<CreateStripePaymentIntentCommand, string>
{
    private readonly IApplicationDbContext _db;
    private readonly IConfiguration _config;

    public CreateStripePaymentIntentCommandHandler(IApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;

        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<string> Handle(CreateStripePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _db.Bookings
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
            throw new AppException("Booking not found");
        
        var paymentDetail = await _db.PaymentDetails.Where(x => x.BookingId == request.BookingId)
            .OrderBy(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);
        if (paymentDetail == null)
            throw new AppException("Payment Detail not found");

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)paymentDetail.Detail.TotalPrice,
            Currency = paymentDetail.Currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "paymentDetailId", paymentDetail.Id.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

        // Save Intent + Update status = Pending
        paymentDetail.StripePaymentIntentId = paymentIntent.Id;

        await _db.SaveChangesAsync(cancellationToken);

        return paymentIntent.ClientSecret;
    }
}