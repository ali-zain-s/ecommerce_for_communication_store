using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;
using MobileShop.Services;

namespace MobileShop.Pages.Products;

public class DetailsModel(ApplicationDbContext db, CartService cart) : PageModel
{
    public Product Product { get; set; } = null!;

    [BindProperty]
    public int Quantity { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        if (product is null) return NotFound();
        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        if (product is null) return NotFound();

        if (Quantity < 1) Quantity = 1;
        await cart.AddAsync(id, Quantity);

        return RedirectToPage("/Cart/Index");
    }
}
