using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductStoreAPI.Models;

namespace ProductStoreAPI.Pages;

public class RegisterPageModel : PageModel
{
    private readonly StoreContext _context;

    [BindProperty] public string RegisterErrorMessage { get; set; } = "";

    public RegisterPageModel(StoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var form = HttpContext.Request.Form;
        // Form will always contain those keys, because of client-side validation.
        var username = form["username"][0]!;
        var password = form["password"][0]!; 
        var role = form["role"][0]!;
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
        {
            RegisterErrorMessage = "User with the same name already exists.";
            return Page();
        }
        user = new StoreUser(username, password, StoreUser.StringToRole(role));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return Redirect("/login");
    }
}