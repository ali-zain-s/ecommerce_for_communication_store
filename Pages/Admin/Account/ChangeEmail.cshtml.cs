using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileShop.Pages.Admin.Account;

public class ChangeEmailModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : PageModel
{
    public string CurrentEmail { get; set; } = string.Empty;

    [BindProperty, Required, EmailAddress]
    public string NewEmail { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return RedirectToPage("/AdminLogin");
        CurrentEmail = user.Email ?? user.UserName ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return RedirectToPage("/AdminLogin");
        CurrentEmail = user.Email ?? user.UserName ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!await userManager.CheckPasswordAsync(user, CurrentPassword))
        {
            ModelState.AddModelError(nameof(CurrentPassword), "That password isn't right.");
            return Page();
        }

        var existing = await userManager.FindByEmailAsync(NewEmail);
        if (existing is not null && existing.Id != user.Id)
        {
            ModelState.AddModelError(nameof(NewEmail), "That email is already in use.");
            return Page();
        }

        // Email doubles as the sign-in username, so keep both in sync.
        await userManager.SetEmailAsync(user, NewEmail);
        await userManager.SetUserNameAsync(user, NewEmail);

        await signInManager.RefreshSignInAsync(user);
        TempData["Message"] = "Your email has been updated.";
        return RedirectToPage();
    }
}
