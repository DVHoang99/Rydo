using Microsoft.EntityFrameworkCore;
using Rydo.Domain.Entities;

namespace Rydo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }
    DbSet<CarImage> CarImages { get; }
    DbSet<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    //dotnet ef migrations add AddBooking -p ../Rydo.Infrastructure -s . 
}