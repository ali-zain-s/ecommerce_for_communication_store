using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileShop.Models;

namespace MobileShop.Data;

public static class SeedData
{
    public const string AdminEmail = "admin@mobileshop.local";
    public const string AdminPassword = "Admin@12345";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser is null)
        {
            adminUser = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, AdminPassword);
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (!await db.Categories.AnyAsync())
        {
            var phones = new Category { Name = "Smartphones" };
            var earbuds = new Category { Name = "Earbuds & Headphones" };
            var chargers = new Category { Name = "Chargers & Cables" };
            var cases = new Category { Name = "Cases & Covers" };
            db.Categories.AddRange(phones, earbuds, chargers, cases);
            await db.SaveChangesAsync();

            db.Products.AddRange(
                new Product
                {
                    Name = "Aurora X12 Smartphone",
                    Description = "6.5\" AMOLED display, 128GB storage, 5000mAh battery, triple camera.",
                    Price = 249999,
                    Stock = 15,
                    CategoryId = phones.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Nova Lite 5G",
                    Description = "Budget-friendly 5G phone with 64GB storage and 48MP camera.",
                    Price = 129999,
                    Stock = 25,
                    CategoryId = phones.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Pulse Wireless Earbuds",
                    Description = "Bluetooth 5.3 earbuds with active noise cancellation and 30-hour battery life.",
                    Price = 8999,
                    Stock = 40,
                    CategoryId = earbuds.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "BassBoost Over-Ear Headphones",
                    Description = "Over-ear headphones with deep bass and foldable design.",
                    Price = 14999,
                    Stock = 20,
                    CategoryId = earbuds.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "FastCharge 65W USB-C Charger",
                    Description = "Compact GaN charger with 65W fast charging for phones and laptops.",
                    Price = 4999,
                    Stock = 60,
                    CategoryId = chargers.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Braided USB-C Cable (2m)",
                    Description = "Durable braided charging and data cable, 2 meters long.",
                    Price = 1499,
                    Stock = 100,
                    CategoryId = chargers.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Shockproof Silicone Case",
                    Description = "Slim shockproof silicone case with raised edges for screen protection.",
                    Price = 1999,
                    Stock = 80,
                    CategoryId = cases.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Clear Tempered Glass Cover",
                    Description = "Transparent hard case with tempered glass back panel.",
                    Price = 2499,
                    Stock = 50,
                    CategoryId = cases.Id,
                    IsActive = true
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
