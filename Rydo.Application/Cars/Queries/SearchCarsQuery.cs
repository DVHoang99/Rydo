using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Cars.Queries;

public record CarDto(
    Guid Id,
    Guid OwnerId,
    string Brand,
    string Model,
    decimal PricePerDay,
    double Latitude,
    double Longitude
);

public record SearchCarsQuery(
    double? Latitude,
    double? Longitude,
    double? MaxDistanceKm,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Brand,
    string? Model
) : IRequest<List<CarDto>>;

public class SearchCarsQueryHandler : IRequestHandler<SearchCarsQuery, List<CarDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly GeometryFactory _geometryFactory;

    public SearchCarsQueryHandler(IApplicationDbContext context, GeometryFactory geometryFactory)
    {
        _context = context;
        _geometryFactory = geometryFactory;
    }

    public async Task<List<CarDto>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Cars.AsQueryable();

        if (request.Latitude.HasValue && request.Longitude.HasValue && request.MaxDistanceKm.HasValue)
        {
            var point = _geometryFactory.CreatePoint(
                new Coordinate(request.Longitude.Value, request.Latitude.Value)
            );

            query = query.Where(c =>
                    c.Location.IsWithinDistance(point, request.MaxDistanceKm.Value * 1000) // km -> meter
            );
        }

        if (request.MinPrice.HasValue)
            query = query.Where(c => c.PricePerDay >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(c => c.PricePerDay <= request.MaxPrice.Value);

        if (!string.IsNullOrEmpty(request.Brand))
            query = query.Where(c => c.Brand.ToLower() == request.Brand.ToLower());

        if (!string.IsNullOrEmpty(request.Model))
            query = query.Where(c => c.Model.ToLower() == request.Model.ToLower());

        return await query
            .Select(c => new CarDto(
                c.Id,
                c.OwnerId,
                c.Brand,
                c.Model,
                c.PricePerDay,
                c.Location.Y, // latitude
                c.Location.X  // longitude
            ))
            .ToListAsync(cancellationToken);
    }
}