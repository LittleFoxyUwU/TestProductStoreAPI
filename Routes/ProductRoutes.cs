using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductStoreAPI.Models;

namespace ProductStoreAPI.Routes;

public static class ProductRoutes
{
    /// <summary>
    /// Sets all product-based routes to the given app
    /// </summary>
    public static void SetRoutes(ref WebApplication app)
    {
        SetGetRoutes(ref app);
        SetPutRoutes(ref app);
        SetPostRoutes(ref app);
        SetDeleteRoutes(ref app);
    }

    private static void SetGetRoutes(ref WebApplication app)
    {
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
    }
    
    private static void SetPutRoutes(ref WebApplication app)
    {
        app.MapPut("/products/{id:int}", async (AppDatabase db, Product product, int id) =>
        {
            var found = await db.Products.FindAsync(id);
            if (found is null) return Results.NotFound();
            found.Name = product.Name;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
    
    private static void SetPostRoutes(ref WebApplication app)
    {
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
    }
    
    private static void SetDeleteRoutes(ref WebApplication app)
    {
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
    }
}