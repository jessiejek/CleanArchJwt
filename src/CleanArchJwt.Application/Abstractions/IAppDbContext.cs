using CleanArchJwt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchJwt.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
