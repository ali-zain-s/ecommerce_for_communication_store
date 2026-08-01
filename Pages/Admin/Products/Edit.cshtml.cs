using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.Models;

namespace MobileShop.Pages.Admin.Products;

public class EditModel(ApplicationDbContext db, IWebHostEnvironment env) : PageModel
{
    public List<Category> Categories { get; set; } = new();
    public string? CurrentImagePath { get; set; }

    [BindProperty]
    public int Id { get; set; }

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
    public bool IsActive { get; set; }

    [BindProperty]
    public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();

        Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        Stock = product.Stock;
        CategoryId = product.CategoryId;
        IsActive = product.IsActive;
        CurrentImagePath = product.ImagePath;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var product = await db.Products.FindAsync(Id);
        if (product is null) return NotFound();

        Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
        CurrentImagePath = product.ImagePath;

        if (!ModelState.IsValid) return Page();

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

        product.Name = Name;
        product.Description = Description;
        product.Price = Price;
        product.Stock = Stock;
        product.CategoryId = CategoryId;
        product.IsActive = IsActive;

        await db.SaveChangesAsync();

        TempData["Message"] = $"'{product.Name}' was updated.";
        return RedirectToPage("/Admin/Products/Index");
    }
}
