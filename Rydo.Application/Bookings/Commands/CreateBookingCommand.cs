using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Interfaces;
using Rydo.Application.Interfaces.Email;
using Rydo.Domain.Entities;

namespace Rydo.Application.Cars.Commands;

public class CreateBookingCommand : IRequest<Guid>
{
    public Guid CarId { get; set; }
    public string UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IEmailService _emailService;

    public CreateBookingCommandHandler(IApplicationDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var overlap = await _db.Bookings.AnyAsync(x =>
            x.CarId == request.CarId &&
            (request.EndDate >= x.StartDate && request.StartDate <= x.EndDate) &&
            (x.Status == "Pending" || x.Status == "Confirmed"), cancellationToken: cancellationToken);

        if (overlap)
            throw new Exception("Car is not available in selected period");
        
        var user = await _db.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.UserId, cancellationToken: cancellationToken);
        if (user == null)
            throw new Exception("User is not found");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            CarId = request.CarId,
            UserId = request.UserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "Pending"
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(cancellationToken);

        var sendEmail = new BookingEmailModel
        {
            CustomerName = user.FirstName + " " + user.LastName,
            CustomerEmail = user.Email,
            BookingTime = DateTime.Now,
            BookingCode = booking.Id.ToString()
        };
        
        await _emailService.SendBookingConfirmationAsync(sendEmail, cancellationToken);

        return booking.Id;
    }
}
