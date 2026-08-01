using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;

namespace MobileShop.Services;

public class CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
{
    private const string SessionKey = "Cart";

    private ISession Session => httpContextAccessor.HttpContext!.Session;

    private List<CartLine> GetLines()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartLine>();
        return JsonSerializer.Deserialize<List<CartLine>>(json) ?? new List<CartLine>();
    }

    private void SaveLines(List<CartLine> lines)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(lines));
    }

    public async Task AddAsync(int productId, int quantity)
    {
        var lines = GetLines();
        var existing = lines.FirstOrDefault(l => l.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            lines.Add(new CartLine { ProductId = productId, Quantity = quantity });
        }
        SaveLines(lines);
        await Task.CompletedTask;
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var lines = GetLines();
        var existing = lines.FirstOrDefault(l => l.ProductId == productId);
        if (existing is null) return;
        if (quantity <= 0)
        {
            lines.Remove(existing);
        }
        else
        {
            existing.Quantity = quantity;
        }
        SaveLines(lines);
    }

    public void Remove(int productId)
    {
        var lines = GetLines();
        lines.RemoveAll(l => l.ProductId == productId);
        SaveLines(lines);
    }

    public void Clear()
    {
        Session.Remove(SessionKey);
    }

    public async Task<List<CartItem>> GetItemsAsync()
    {
        var lines = GetLines();
        if (lines.Count == 0) return new List<CartItem>();

        var productIds = lines.Select(l => l.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        var items = new List<CartItem>();
        foreach (var line in lines)
        {
            var product = products.FirstOrDefault(p => p.Id == line.ProductId);
            if (product is null || !product.IsActive) continue;
            items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                ImagePath = product.ImagePath,
                Quantity = line.Quantity
            });
        }
        return items;
    }

    public async Task<int> GetItemCountAsync()
    {
        var items = await GetItemsAsync();
        return items.Sum(i => i.Quantity);
    }
}
