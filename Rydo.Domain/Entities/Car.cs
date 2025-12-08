using NetTopologySuite.Geometries;

namespace Rydo.Domain.Entities;

public class Car
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Seats { get; set; }

    public string Transmission { get; set; } = "Automatic"; // Manual
    public string Fuel { get; set; } = "Gasoline"; // Diesel, Electric

    public decimal PricePerDay { get; set; }

    public bool IsActive { get; set; } = true;

    public string Address { get; set; } = string.Empty; // ví dụ: 123 Nguyễn Trãi, Hà Nội
    public Point Location { get; set; } = default!;     // Geo-Spatial (longitude, latitude)

    public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
    public ICollection<Booking> Bookings { get; set; }
}