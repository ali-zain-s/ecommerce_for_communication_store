using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
