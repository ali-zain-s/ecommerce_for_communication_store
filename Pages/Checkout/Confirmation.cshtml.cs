using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Checkout;

public class ConfirmationModel(ApplicationDbContext db) : PageModel
{
    public Order Order { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await db.Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();
        Order = order;
        return Page();
    }
}
