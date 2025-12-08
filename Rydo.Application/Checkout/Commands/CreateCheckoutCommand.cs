using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;

namespace Rydo.Application.Checkout.Commands;

public record CreateCheckoutCommand(Guid BookingId, CheckoutType CheckoutType, PaymentStatus PaymentStatus) : IRequest<Guid>;

public class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public CreateCheckoutCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var booking = await _db.Bookings
            .Include(b => b.Car)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        if (booking.Status != "PendingPayment" && booking.Status != "Draft")
            throw new Exception("Booking already checked out");

        var pricePerDay = booking.Car.PricePerDay;
        var totalDays = (int)(booking.EndDate.Date - booking.StartDate.Date).TotalDays;
        if (totalDays <= 0) throw new Exception("Invalid booking dates");

        decimal discount = 0; // apply logic later
        var totalPrice = (pricePerDay * totalDays) - discount;

        var detail = new Detail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            TotalDays = totalDays,
            PricePerDay = pricePerDay,
            Discount = discount,
            TotalPrice = totalPrice,
        };
        
        var paymentDetail = new PaymentDetail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            Status = request.PaymentStatus,
            CheckoutType = request.CheckoutType,
            Detail = detail
        };

        await _db.PaymentDetails.AddAsync(paymentDetail, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return paymentDetail.Id;
    }
}
