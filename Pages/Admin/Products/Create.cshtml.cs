using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Admin.Products;

public class CreateModel(ApplicationDbContext db, IWebHostEnvironment env) : PageModel
{
    public List<Category> Categories { get; set; } = new();

    [BindProperty, Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [BindProperty, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [BindProperty, Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [BindProperty, Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [BindProperty, Required]
    public int CategoryId { get; set; }

    [BindProperty]
    public IFormFile? Image { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();

        if (!ModelState.IsValid) return Page();

        var product = new Product
        {
            Name = Name,
            Description = Description,
            Price = Price,
            Stock = Stock,
            CategoryId = CategoryId,
            IsActive = true
        };

        if (Image is not null && Image.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError(nameof(Image), "Only JPG, PNG, or WEBP images are allowed.");
                return Page();
            }

            var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }
            product.ImagePath = $"uploads/products/{fileName}";
        }

        db.Products.Add(product);
        await db.SaveChangesAsync();

        TempData["Message"] = $"'{product.Name}' was added.";
        return RedirectToPage("/Admin/Products/Index");
    }
}
