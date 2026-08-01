using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileShop.Areas.Identity.Pages.Account;

public class ResendEmailConfirmationModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/AdminLogin");
    public IActionResult OnPost() => RedirectToPage("/AdminLogin");
}
