namespace Rydo.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(BookingEmailModel model, CancellationToken cancellationToken);
}

public class BookingEmailModel
{
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public DateTime BookingTime { get; set; }
    public string BookingCode { get; set; } = default!;
}