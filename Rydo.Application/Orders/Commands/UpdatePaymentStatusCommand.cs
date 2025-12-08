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
        return request.PaymentId;
    }
}