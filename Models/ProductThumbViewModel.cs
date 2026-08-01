namespace MobileShop.Models;

public class ProductThumbViewModel
{
    public string? ImagePath { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CssClass { get; set; } = "ms-product-thumb";
}
