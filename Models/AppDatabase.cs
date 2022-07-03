using Microsoft.EntityFrameworkCore;

namespace ProductStoreAPI.Models;

public class AppDatabase : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    
    public DbSet<User> Users { get; set; } = null!;

    public AppDatabase(DbContextOptions options) : base(options) {Database.EnsureCreated();}
}