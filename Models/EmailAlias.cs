namespace EmailAliasApi.Models;

public class EmailAlias
{
    public int Id { get; set; }
    public string AliasAddress { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<EmailForwarding> ForwardingAddresses { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
} 