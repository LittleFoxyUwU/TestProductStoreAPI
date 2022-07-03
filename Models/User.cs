using System.ComponentModel;

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