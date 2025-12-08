

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Checkout.Commands;
using Rydo.Application.Common.Enums;
using Rydo.Domain.Entities;
using Rydo.Infrastructure.Persistence;
using Xunit;

namespace Rydo.Appication.Test;

public class CreateCheckoutCommandHandlerTests
{
    private readonly ApplicationDbContext _db;
    private readonly CreateCheckoutCommandHandler _handler;

    public CreateCheckoutCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _handler = new CreateCheckoutCommandHandler(_db);
    }

    [Fact]
    public async Task Should_Create_PaymentDetail_When_Valid()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Car = new Car { Id = Guid.NewGuid(), PricePerDay = 100 },
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            Status = "Draft"
        };

        await _db.Bookings.AddAsync(booking);
        await _db.SaveChangesAsync();

        var command = new CreateCheckoutCommand(booking.Id, CheckoutType.Offline, PaymentStatus.Pending);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var payment = await _db.PaymentDetails.FirstOrDefaultAsync(x => x.Id == resultId);
        payment.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Throw_When_Booking_Not_Found()
    {
        var command = new CreateCheckoutCommand(Guid.NewGuid(), CheckoutType.Offline, PaymentStatus.Pending);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Booking not found");
    }

    [Fact]
    public async Task Should_Calculate_Correct_Price()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Car = new Car { Id = Guid.NewGuid(), PricePerDay = 200 },
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            Status = "Draft"
        };

        await _db.Bookings.AddAsync(booking);
        await _db.SaveChangesAsync();

        var command = new CreateCheckoutCommand(booking.Id, CheckoutType.Offline, PaymentStatus.Pending);
        await _handler.Handle(command, CancellationToken.None);

        var detail = await _db.Set<Detail>().FirstAsync();

        detail.PricePerDay.Should().Be(200);
        detail.TotalDays.Should().Be(2);
        detail.TotalPrice.Should().Be(400);
    }
}