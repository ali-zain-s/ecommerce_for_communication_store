using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Orders;

public class TrackModel(ApplicationDbContext db) : PageModel
{
    [BindProperty, Required]
    public int OrderId { get; set; }

    [BindProperty, Required]
    public string Phone { get; set; } = string.Empty;

    public Order? Order { get; set; }
    public bool Searched { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        Searched = true;
        Order = await db.Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == OrderId && o.CustomerPhone == Phone);
    }
}
