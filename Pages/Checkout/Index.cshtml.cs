using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;
using MobileShop.Services;

namespace MobileShop.Pages.Checkout;

public class IndexModel(ApplicationDbContext db, CartService cart, IWebHostEnvironment env) : PageModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.LineTotal);

    [BindProperty]
    [Required(ErrorMessage = "Please enter your name")]
    [StringLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Please enter your phone number")]
    [StringLength(30)]
    public string CustomerPhone { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Please enter a delivery address")]
    [StringLength(400)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    [BindProperty]
    public IFormFile? PaymentProof { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Items = await cart.GetItemsAsync();
        if (Items.Count == 0)
        {
            return RedirectToPage("/Cart/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Items = await cart.GetItemsAsync();
        if (Items.Count == 0)
        {
            return RedirectToPage("/Cart/Index");
        }

        if (PaymentMethod == PaymentMethod.Online && (PaymentProof is null || PaymentProof.Length == 0))
        {
            ModelState.AddModelError(nameof(PaymentProof), "Please upload a screenshot of your payment for online payments.");
        }

        if (PaymentProof is not null && PaymentProof.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(PaymentProof.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError(nameof(PaymentProof), "Only JPG, PNG, or WEBP images are allowed.");
            }
            else if (PaymentProof.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(PaymentProof), "Image must be under 5 MB.");
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var productIds = Items.Select(i => i.ProductId).ToList();
        var products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        var order = new Order
        {
            CustomerName = CustomerName,
            CustomerPhone = CustomerPhone,
            DeliveryAddress = DeliveryAddress,
            PaymentMethod = PaymentMethod,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0;
        foreach (var item in Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is null || !product.IsActive || product.Stock < item.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"'{item.ProductName}' is no longer available in the requested quantity.");
                return Page();
            }

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            });
            product.Stock -= item.Quantity;
            total += product.Price * item.Quantity;
        }
        order.TotalAmount = total;

        if (PaymentMethod == PaymentMethod.Online && PaymentProof is not null && PaymentProof.Length > 0)
        {
            var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "proofs");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(PaymentProof.FileName).ToLowerInvariant()}";
            var filePath = Path.Combine(uploadsDir, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await PaymentProof.CopyToAsync(stream);
            }
            order.PaymentProofPath = $"uploads/proofs/{fileName}";
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        cart.Clear();

        return RedirectToPage("/Checkout/Confirmation", new { id = order.Id });
    }
}
