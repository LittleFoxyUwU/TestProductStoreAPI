using Microsoft.EntityFrameworkCore;

namespace ProductStoreAPI.Models;

public class ProductDb : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public ProductDb(DbContextOptions options) : base(options) {}
}