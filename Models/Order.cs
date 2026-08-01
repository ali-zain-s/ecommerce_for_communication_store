using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShop.Models;

public class Order
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required, StringLength(400)]
    public string DeliveryAddress { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public string? PaymentProofPath { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>Admin has manually confirmed the buyer is real (COD) or reviewed the payment proof (Online).</summary>
    public bool AdminVerified { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
