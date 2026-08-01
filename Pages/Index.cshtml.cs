using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages;

public class IndexModel(ApplicationDbContext db) : PageModel
{
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();

        var query = db.Products.Include(p => p.Category).Where(p => p.IsActive);
        if (CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == CategoryId.Value);
        }
        Products = await query.OrderBy(p => p.Name).ToListAsync();
    }
}
