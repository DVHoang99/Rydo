namespace Rydo.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Guid UserId { get; set; }
    public string CustomerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Pending";

    public Car Car { get; set; }
    public List<PaymentDetail> PaymentDetails { get; set; }
}