using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProductStoreAPI;
using ProductStoreAPI.Models;

var app = WebApiApp.AppInitialisator(args);

#region Login and User Routes

app.MapGet("/login_check", async (StoreContext db, HttpContext context) =>
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

app.MapGet("/products", async (StoreContext db, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    var products = await db.Products.ToListAsync();
    return products;
});

app.MapGet("/products/{id:int}", async (StoreContext db, int id, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    return await db.Products.FindAsync(id);
});

app.MapGet("/products/{name}", async (StoreContext db, string name, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    return await db.Products.FindAsync(name);
});

app.MapPost("/products",  async (StoreContext db, Product product) =>
{
    if (db.Products.Any(p => p.Id == product.Id))
    {
        return Results.Problem($"The product with id: {product.Id} already exists.");
    }
  
    await db.Products.AddAsync(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:int}", async (StoreContext db, Product product, int id) =>
{
    var found = await db.Products.FindAsync(id);
    if (found is null) return Results.NotFound();
    found.Name = product.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/products/{id:int}", async (StoreContext db, int id) =>
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

