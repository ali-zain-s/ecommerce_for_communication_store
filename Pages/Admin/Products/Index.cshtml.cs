using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Admin.Products;

public class IndexModel(ApplicationDbContext db) : PageModel
{
    public List<Product> Products { get; set; } = new();
    public int ActiveCount { get; set; }
    public int HiddenCount { get; set; }
    public int LowStockCount { get; set; }

    public async Task OnGetAsync()
    {
        Products = await db.Products.Include(p => p.Category)
            .OrderByDescending(p => p.IsActive)
            .ThenBy(p => p.Name)
            .ToListAsync();

        ActiveCount = Products.Count(p => p.IsActive);
        HiddenCount = Products.Count(p => !p.IsActive);
        LowStockCount = Products.Count(p => p.IsActive && p.Stock <= 5);
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.IsActive = !product.IsActive;
        await db.SaveChangesAsync();
        TempData["Message"] = product.IsActive ? $"'{product.Name}' is now visible in the store." : $"'{product.Name}' has been hidden from the store.";
        return RedirectToPage();
    }
}
