using Rydo.Application.Common.Enums;

namespace Rydo.Domain.Entities;

public class PaymentDetail
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public DateTime Created { get; set; }
    public PaymentStatus Status { get; set; }
    public CheckoutType CheckoutType { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripePaymentMethodId { get; set; }

    public decimal? PaidAmount { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Currency { get; set; } = "USD";
    public Detail Detail { get; set; }
}

public class Detail
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public DateTime Created { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal PricePerDay { get; set; }
    public int TotalDays { get; set; }
}