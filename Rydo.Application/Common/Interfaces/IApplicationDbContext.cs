using Microsoft.EntityFrameworkCore;
using Rydo.Domain.Entities;

namespace Rydo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }
    DbSet<CarImage> CarImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}