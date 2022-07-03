namespace ProductStoreAPI.Models;

public class Product
{
    public int Id { get; init; }

    public string Name { get; set; }

    public Product(int id, string name)
    {
        Id = id;
        Name = name;
    }
}