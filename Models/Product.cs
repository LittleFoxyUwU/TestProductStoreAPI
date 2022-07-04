namespace ProductStoreAPI.Models;

public class Product
{
    public int Id { get; init; }
    
    public string Name { get; set; }
    
    public double Price { get; set; }

    public Product(string name, double price)
    {
        Name = name;
        Price = price;
    }
}