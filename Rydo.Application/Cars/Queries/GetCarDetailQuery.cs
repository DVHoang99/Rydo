using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;

namespace Rydo.Application.Cars.Queries;

public record GetCarDetailQuery(Guid CarId) : IRequest<Car>;

public class GetCarDetailQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCarDetailQuery, Car>
{
    public async Task<Car> Handle(GetCarDetailQuery request, CancellationToken cancellationToken)
    {
        var car = await db.Cars.FirstOrDefaultAsync(x => x.Id == request.CarId, cancellationToken: cancellationToken);
        if (car == null) throw new Exception("Car not found");

        return car;
    }
}