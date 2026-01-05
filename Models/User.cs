namespace EmailAliasApi.Models;

public class User
{
    public enum UserRole
    {
        USER,
        ADMIN
    }

    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.USER;
    public List<EmailForwarding> ForwardingAddresses { get; set; } = new();
} 