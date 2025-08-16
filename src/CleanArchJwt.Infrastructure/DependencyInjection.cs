using CleanArchJwt.Application.Abstractions;
using CleanArchJwt.Application.Auth;
using CleanArchJwt.Infrastructure.Auth;
using CleanArchJwt.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchJwt.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ✅ Match the key in appsettings.json ("DefaultConnection")
        var connectionString = config.GetConnectionString("DefaultConnection");

        // ✅ Use SQL Server provider
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // DbContext abstraction
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Auth & JWT
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
