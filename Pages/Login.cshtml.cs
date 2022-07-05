using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductStoreAPI.Models;

namespace ProductStoreAPI.Pages;

[IgnoreAntiforgeryToken]
public class LoginPageModel : PageModel
{
    private readonly StoreContext _context;

    [BindProperty] public string LoginErrorMessage { get; set; } = "";

    public LoginPageModel(StoreContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var form = HttpContext.Request.Form;
        // Form will always contain username and password because of the client side validation
        var username = form["username"][0]!; 
        var password = form["password"][0]!;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    
        if (user == null)
        {
            LoginErrorMessage = "Invalid username or password";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString()!)
        };
        
        var identity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
        return Redirect("/");
    }
}