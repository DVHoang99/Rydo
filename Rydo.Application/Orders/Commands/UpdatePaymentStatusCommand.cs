using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Cars.Queries;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Orders.Commands;

public record UpdatePaymentStatusCommand(string PaymentId, PaymentStatus Status) : IRequest<string>;

public class UpdatePaymentStatusCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdatePaymentStatusCommand, string>
{
    private readonly IApplicationDbContext _db = db;

    public async Task<string> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PaymentId, out var paymentGuid))
            throw new ArgumentException("Invalid Payment Id");

        var payment = await _db.PaymentDetails
            .FirstOrDefaultAsync(x => x.Id == paymentGuid, cancellationToken);

        if (payment == null)
            return $"Payment {request.PaymentId} not found.";

        // Update status
        payment.Status = request.Status;

        await _db.SaveChangesAsync(cancellationToken);

        return request.PaymentId;
    }
}