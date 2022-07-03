using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProductStoreAPI;
using ProductStoreAPI.Models;

var app = WebApiApp.AppInitialisator(args);

#region Login and User Routes

// Login via GET: /login form page 
app.MapPost("/login", async (AppDatabase db, HttpContext context) =>
{
    var form = context.Request.Form;
    if (!form.ContainsKey("username") || !form.ContainsKey("password"))
    {
        return Results.BadRequest("Missing username or password");
    }
    var username = form["username"][0]!;
    var password = form["password"][0]!;

    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    
    if (user == null) return Results.Unauthorized();

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role.ToString())
    };
        
    var identity = new ClaimsIdentity(claims, "Cookies");
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity));
    return Results.Redirect("/");
});

app.MapPost("/register", async (AppDatabase db, HttpContext context) =>
{
    var form = context.Request.Form;
    if (!form.ContainsKey("username") || !form.ContainsKey("password"))
    {
        return Results.BadRequest("Missing username or password");
    }
    var username = form["username"][0]!;
    var password = form["password"][0]!;
    var role = form["role"][0]!;
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user != null) return Results.BadRequest("User with the same name already exists");
    user = new User { Username = username, Password = password, Role = User.StringToRole(role)};
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return Results.Redirect("/login");
});

app.MapGet("/login_check", async (AppDatabase db, HttpContext context) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == context.User.Identity.Name);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(user);
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

#endregion

#region Product Routes

app.MapGet("/products", async (AppDatabase db, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    var products = await db.Products.ToListAsync();
    return products;
});

app.MapGet("/products/{id:int}", async (AppDatabase db, int id, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    return await db.Products.FindAsync(id);
});

app.MapGet("/products/{name}", async (AppDatabase db, string name, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    return await db.Products.FindAsync(name);
});

app.MapPost("/products",  async (AppDatabase db, Product product) =>
{
    if (db.Products.Any(p => p.Id == product.Id))
    {
        return Results.Problem($"The product with id: {product.Id} already exists.");
    }
  
    await db.Products.AddAsync(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:int}", async (AppDatabase db, Product product, int id) =>
{
    var found = await db.Products.FindAsync(id);
    if (found is null) return Results.NotFound();
    found.Name = product.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/products/{id:int}", async (AppDatabase db, int id) =>
{
    var found = await db.Products.FindAsync(id);
    if (found is null)
    {
        return Results.NotFound();
    }

    db.Products.Remove(found);
    await db.SaveChangesAsync();
    return Results.Ok();
});

#endregion

app.Run();

