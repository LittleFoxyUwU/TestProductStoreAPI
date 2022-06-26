using Microsoft.OpenApi.Models;
using ProductStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

var options = new WebApplicationOptions
{
    Args = args,
    ApplicationName = "ProductStoreAPI"
};
var builder = WebApplication.CreateBuilder(options);
var connectionString = builder.Configuration.GetConnectionString("Products") ?? "Data Source=Products.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<ProductDb>(connectionString);
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddSwaggerGen(c =>
{  
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProductStoreAPI",
        Description = "Making the Products you love", 
        Version = "v1" 
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductStoreAPI API V1");
});

app.MapGet("/", async (HttpRequest request, HttpContext context) =>
{
    var path = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(request);
    context.Response.ContentType = "text/html; charset=utf-8";
    var responseBody = $"<html><body><h1>Welcome to Foxy's Products Store!</h1><a href='{path + "swagger"}'>Please refer to Swagger</a></body></html>";
    return responseBody;
});

app.MapGet("/products", async (ProductDb db, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    var products = await db.Products.ToListAsync();
    return products;
});

app.MapGet("/products/{id:int}", async (ProductDb db, int id, HttpContext context) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        return await db.Products.FindAsync(id);
    });

app.MapGet("/products/{name}", async (ProductDb db, string name, HttpContext context) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    return await db.Products.FindAsync(name);
});

app.MapPost("/products", async (ProductDb db, Product product) =>
{
    if (db.Products.Any(p => p.Id == product.Id))
    {
        return Results.Problem($"The product with id: {product.Id} already exists.");
    }
  
    await db.Products.AddAsync(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:int}", async (ProductDb db, Product product, int id) =>
{
    var found = await db.Products.FindAsync(id);
    if (found is null) return Results.NotFound();
    found.Name = product.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/products/{id:int}", async (ProductDb db, int id) =>
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

app.Run();

