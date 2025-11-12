namespace AuthenticationApi.Domain.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? HashedPassword { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? Role { get; set; }
    public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
}
