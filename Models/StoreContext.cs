using Microsoft.EntityFrameworkCore;

namespace ProductStoreAPI.Models;

public class StoreContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    
    public DbSet<StoreUser> Users { get; set; } = null!;

    public StoreContext(DbContextOptions options) : base(options) {Database.EnsureCreated();}
}