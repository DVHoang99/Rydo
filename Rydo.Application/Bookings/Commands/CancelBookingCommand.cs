using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Algorithm.Distance;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Bookings.Commands;

public class CancelBookingCommand(Guid bookingId) : IRequest<Guid>
{
    public Guid BookingId { get; set; } = bookingId;
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CancelBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == request.BookingId && x.Status == "Pending", cancellationToken);
        
        if (booking == null)
            throw new Exception($"Booking {request.BookingId} not found");
        
        booking.Status = "Cancelled";
        await _context.SaveChangesAsync(cancellationToken);
        
        return request.BookingId;
    }
}