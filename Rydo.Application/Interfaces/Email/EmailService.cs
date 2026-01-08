using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Rydo.Application.Interfaces.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendBookingConfirmationAsync(BookingEmailModel model, CancellationToken cancellationToken)
    {
        var subject = $"Booking Confirmation - {model.BookingCode}";
        var body = BuildBookingEmailBody(model);

        if (_smtp.From != null)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtp.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(model.CustomerEmail);

            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.UserName, _smtp.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }

    private static string BuildBookingEmailBody(BookingEmailModel model)
    {
        return $@"
            <h2>Booking Confirmation</h2>
            <p>Dear <b>{model.CustomerName}</b>,</p>

            <p>Your booking has been successfully confirmed with the following details:</p>

            <ul>
                <li><b>Date & Time:</b> {model.BookingTime:dd/MM/yyyy HH:mm}</li>
                <li><b>Booking Code:</b> {model.BookingCode}</li>
            </ul>

            <p>If you have any questions, please contact us.</p>

            <p>Best regards,<br/>
            Booking System</p>
        ";
    }
}