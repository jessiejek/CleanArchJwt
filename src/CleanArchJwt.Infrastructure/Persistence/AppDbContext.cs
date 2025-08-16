using CleanArchJwt.Application.Abstractions;
using CleanArchJwt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchJwt.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Email).IsRequired();
            b.Property(u => u.PasswordHash).IsRequired();
            b.Property(u => u.Role).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasIndex(rt => rt.Token).IsUnique();
            b.Property(rt => rt.Token).IsRequired();
            b.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
        });
    }
}
