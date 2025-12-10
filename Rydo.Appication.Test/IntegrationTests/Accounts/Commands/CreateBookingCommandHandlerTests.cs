using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Rydo.Appication.Test.Helpers;
using Rydo.Application.Cars.Commands;
using Rydo.Domain.Entities;
using Xunit;

namespace Rydo.Appication.Test.IntegrationTests.Accounts.Commands;

public class CreateBookingCommandHandlerTests : IntegrationTestBase
{
    private readonly TestDbContextFactory _factory;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests(TestDbContextFactory factory)
        : base(factory)
    {
        _factory = factory;
        _handler = new CreateBookingCommandHandler(factory.DbContext);
    }
    
    
    [Fact]
    public async Task Should_Create_Booking_When_No_Overlap()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var command = new CreateBookingCommand
        {
            CarId = carId,
            UserId = userId,
            StartDate = new DateTime(2025, 03, 10),
            EndDate = new DateTime(2025, 03, 12)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var booking = await _factory.DbContext.Bookings.FirstOrDefaultAsync(x => x.Id == result);
        booking.Should().NotBeNull();
        booking!.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task Should_Throw_When_Overlap_Exists()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // existing booking
        _factory.DbContext.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CarId = carId,
            UserId = userId,
            StartDate = new DateTime(2025, 03, 10),
            EndDate = new DateTime(2025, 03, 12),
            Status = "Confirmed"
        });
        await _factory.DbContext.SaveChangesAsync();

        // new overlapping booking request
        var command = new CreateBookingCommand
        {
            CarId = carId,
            UserId = Guid.NewGuid(),
            StartDate = new DateTime(2025, 03, 11),
            EndDate = new DateTime(2025, 03, 13)
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Car is not available in selected period");
    }
}