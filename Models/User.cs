namespace ProductStoreAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public StoreRole Role { get; set; }
    
    
    public enum StoreRole
    {
        Admin,
        Worker, 
        User
    }
}