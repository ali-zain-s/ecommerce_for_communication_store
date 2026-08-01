using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Admin.Orders;

public class IndexModel(ApplicationDbContext db) : PageModel
{
    public List<Order> Orders { get; set; } = new();
    public int PendingCount { get; set; }
    public int AwaitingVerificationCount { get; set; }
    public decimal TotalRevenue { get; set; }

    [BindProperty(SupportsGet = true)]
    public OrderStatus? StatusFilter { get; set; }

    public async Task OnGetAsync()
    {
        var allOrders = await db.Orders.Include(o => o.OrderItems).ToListAsync();
        PendingCount = allOrders.Count(o => o.Status == OrderStatus.Pending);
        AwaitingVerificationCount = allOrders.Count(o => !o.AdminVerified && o.Status != OrderStatus.Cancelled);
        TotalRevenue = allOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount);

        Orders = StatusFilter.HasValue
            ? allOrders.Where(o => o.Status == StatusFilter.Value).OrderByDescending(o => o.CreatedAt).ToList()
            : allOrders.OrderByDescending(o => o.CreatedAt).ToList();
    }
}
