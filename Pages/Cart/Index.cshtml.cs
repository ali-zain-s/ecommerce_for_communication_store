using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MobileShop.Services;

namespace MobileShop.Pages.Cart;

public class IndexModel(CartService cart) : PageModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.LineTotal);

    public async Task OnGetAsync()
    {
        Items = await cart.GetItemsAsync();
    }

    public IActionResult OnPostUpdate(int productId, int quantity)
    {
        cart.UpdateQuantity(productId, quantity);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int productId)
    {
        cart.Remove(productId);
        return RedirectToPage();
    }
}
