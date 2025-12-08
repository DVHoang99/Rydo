using MediatR;
using NetTopologySuite.Geometries;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;

namespace Rydo.Application.Cars.Commands;

public record CreateCarCommand(
    Guid OwnerId,
    string Brand,
    string Model,
    int Year,
    int Seats,
    string Transmission,
    string Fuel,
    decimal PricePerDay,
    string Address,
    double Latitude,
    double Longitude
) : IRequest<Guid>;

public class CreateCarCommandHandler(
    IApplicationDbContext context,
    GeometryFactory geometryFactory)
    : IRequestHandler<CreateCarCommand, Guid>
{
    public async Task<Guid> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var location = geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

        var car = new Car
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Seats = request.Seats,
            Transmission = request.Transmission,
            Fuel = request.Fuel,
            PricePerDay = request.PricePerDay,
            Address = request.Address,
            Location = location
        };

        context.Cars.Add(car);
        await context.SaveChangesAsync(cancellationToken);

        return car.Id;
    }
}