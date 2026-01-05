using Microsoft.EntityFrameworkCore;
using EmailAliasApi.Models;

namespace EmailAliasApi.Data;

public class EmailAliasDbContext : DbContext
{
    public EmailAliasDbContext(DbContextOptions<EmailAliasDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<EmailAlias> EmailAliases { get; set; }
    public DbSet<EmailForwarding> EmailForwardings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailForwarding>()
            .HasOne(f => f.EmailAlias)
            .WithMany(e => e.ForwardingAddresses)
            .HasForeignKey(f => f.EmailAliasId);

        modelBuilder.Entity<EmailForwarding>()
            .HasOne(f => f.User)
            .WithMany(u => u.ForwardingAddresses)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
} 