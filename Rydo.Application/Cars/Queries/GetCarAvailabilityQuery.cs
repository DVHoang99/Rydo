using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Cars.Queries;

public record GetCarAvailabilityQuery(Guid CarId, DateTime From, DateTime To) : IRequest<bool>;

public class GetCarAvailabilityQueryHandler : IRequestHandler<GetCarAvailabilityQuery, bool>
{
    private readonly IApplicationDbContext _db;
    public GetCarAvailabilityQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(GetCarAvailabilityQuery request, CancellationToken cancellationToken)
    {
        return !await _db.Bookings
            .AnyAsync(x => x.CarId == request.CarId &&
                           (request.To >= x.StartDate && request.From <= x.EndDate) &&
                           (x.Status == "Pending" || x.Status == "Confirmed"), cancellationToken: cancellationToken);
    }
}