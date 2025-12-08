using Xunit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rydo.Application.Checkout.Commands;
using Rydo.Application.Checkout.Commands;
using Rydo.Application.Common.Enums;

public class CreateCheckoutTests : IntegrationTestBase
{
    [Fact]
    public async Task Should_Create_Checkout_Successfully()
    {
        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateCheckoutCommand(
            BookingId: Guid.NewGuid(),
            CheckoutType: CheckoutType.Offline,
            PaymentStatus: PaymentStatus.Pending
        );

        var result = await mediator.Send(command);

        Assert.NotNull(result);
        Assert.True(result != Guid.Empty);
    }
}