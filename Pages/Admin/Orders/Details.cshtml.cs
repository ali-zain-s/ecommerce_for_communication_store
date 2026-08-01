using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Admin.Orders;

public class DetailsModel(ApplicationDbContext db) : PageModel
{
    public Order Order { get; set; } = null!;

    [BindProperty]
    public OrderStatus NewStatus { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();
        Order = order;
        NewStatus = order.Status;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id)
    {
        var order = await db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();

        order.Status = NewStatus;
        await db.SaveChangesAsync();

        Order = order;
        TempData["Message"] = $"Order #{order.Id} status updated to {order.Status}.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostToggleVerifiedAsync(int id)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();

        order.AdminVerified = !order.AdminVerified;
        await db.SaveChangesAsync();

        TempData["Message"] = order.AdminVerified
            ? $"Order #{order.Id} marked as verified."
            : $"Order #{order.Id} marked as not yet verified.";
        return RedirectToPage(new { id });
    }
}
