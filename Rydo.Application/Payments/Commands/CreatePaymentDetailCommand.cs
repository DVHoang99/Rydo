using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;

namespace Rydo.Application.Payments.Commands;

public record CreatePaymentDetailCommand(Guid BookingId, CheckoutType Type) : IRequest<Guid>;

public class CreatePaymentDetailCommandHandler 
    : IRequestHandler<CreatePaymentDetailCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public CreatePaymentDetailCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreatePaymentDetailCommand request, CancellationToken cancellationToken)
    {
        var booking = await _db.Bookings
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        // Tính toán giá tiền
        var totalDays = (booking.EndDate - booking.StartDate).Days;
        if (totalDays <= 0) totalDays = 1;

        var pricePerDay = booking.Car.PricePerDay;
        var totalPrice = totalDays * pricePerDay;

        // Tạo chi tiết giá
        var detail = new Detail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            TotalPrice = totalPrice,
            TotalDays = totalDays,
            PricePerDay = pricePerDay,
            Discount = 0
        };

        var payment = new PaymentDetail
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Created = DateTime.UtcNow,
            Status = request.Type == CheckoutType.Offline ? PaymentStatus.Successful : PaymentStatus.Pending,
            CheckoutType = request.Type,
            Currency = "usd",
            Detail = detail
        };

        _db.PaymentDetails.Add(payment);
        await _db.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}