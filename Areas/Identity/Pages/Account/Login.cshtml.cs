using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileShop.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    public IActionResult OnGet(string? returnUrl = null) => RedirectToPage("/AdminLogin", new { returnUrl });
    public IActionResult OnPost(string? returnUrl = null) => RedirectToPage("/AdminLogin", new { returnUrl });
}
