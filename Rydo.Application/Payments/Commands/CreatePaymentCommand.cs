using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;

namespace Rydo.Application.Payments.Commands;

public class CreatePaymentCommand : IRequest<Guid>
{
    public Guid BookingId { get; set; }
    public CheckoutType CheckoutType { get; set; }
}

public class CreatePaymentCommandHandler(IApplicationDbContext db) : IRequestHandler<CreatePaymentCommand, Guid>
{
    public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await db.Bookings
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        // Calculate detail
        var totalDays = (booking.EndDate - booking.StartDate).Days + 1;
        decimal pricePerDay = booking.Car.PricePerDay;
        decimal discount = 0;
        decimal totalPrice = (totalDays * pricePerDay) - discount;

        var detail = new Detail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            TotalDays = totalDays,
            PricePerDay = pricePerDay,
            Discount = discount,
            TotalPrice = totalPrice
        };

        var payment = new PaymentDetail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            Status = request.CheckoutType == CheckoutType.Offline ? PaymentStatus.Successful : PaymentStatus.Pending,
            CheckoutType = request.CheckoutType,
            Detail = detail
        };

        db.PaymentDetails.Add(payment);
        await db.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}