using MediatR;

namespace Rydo.Application.Payments.Commands;

public class CheckoutOfflineCommand(Guid BookingId) : IRequest<Guid>;

public class CheckoutOfflineCommandImpl(Guid BookingId) : CheckoutOfflineCommand(BookingId);