namespace EmailAliasApi.Models;

public class EmailForwarding
{
    public int Id { get; set; }
    public string ForwardingAddress { get; set; } = string.Empty;
    public int EmailAliasId { get; set; }
    public EmailAlias EmailAlias { get; set; } = null!;
    public int? UserId { get; set; }
    public User? User { get; set; }
} 