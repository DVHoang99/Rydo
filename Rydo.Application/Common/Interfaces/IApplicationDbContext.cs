using Microsoft.EntityFrameworkCore;
using Rydo.Domain.Entities;

namespace Rydo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }
    DbSet<CarImage> CarImages { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<PaymentDetail> PaymentDetails { get; }
    DbSet<User> Users { get; }
    

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    // example
    //dotnet ef migrations add AddBooking -p ../Rydo.Infrastructure -s . 
    //dotnet ef database update -p ../Rydo.Infrastructure -s .
}