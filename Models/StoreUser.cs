namespace ProductStoreAPI.Models;

public class StoreUser
{
    public int Id { get; init; }
    public string Username { get; set; }
    public string Password { get; set; }
    public StoreRole? Role { get; set; }
    
    public StoreUser(string username, string password, StoreRole? role = StoreRole.Worker)
    {
        Username = username;
        Password = password;
        Role = role;
    }
    
    public enum StoreRole
    {
        Admin,
        Worker,
        Customer
    }

    public static StoreRole StringToRole(string s) => s.ToLowerInvariant() switch
    {
        "admin" => StoreRole.Admin,
        "worker" => StoreRole.Worker,
        "customer" => StoreRole.Customer,
        _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
    };
}