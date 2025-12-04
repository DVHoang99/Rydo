namespace Rydo.Domain.Entities;

public class CarImage
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; }
}