using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serfinsa.Application.Interfaces;
using Serfinsa.Domain.Interfaces;
using Serfinsa.Infrastructure.Data;
using Serfinsa.Infrastructure.Repositories;
using Serfinsa.Infrastructure.Services;

namespace Serfinsa.Infrastructure;

/// <summary>
/// Extensión de IServiceCollection para registrar todos los servicios de Infrastructure.
/// Configuración de EF Core, repositorios, servicios de seguridad y autenticación JWT.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar EF Core con SQL Server usando la cadena de conexión de appsettings
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                // Indica a EF Core las migraciones están en el proyecto Infrastructure
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // Registrar repositorios (implementaciones de las interfaces del Dominio)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Registrar servicios de seguridad
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Configurar autenticación JWT Bearer
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey no está configurada.");

        services.AddAuthentication(options =>
        {
            // Esquema de autenticación por defecto: JWT Bearer
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,          // Verifica que el token no haya expirado
                ValidateIssuerSigningKey = true,   // Verifica la firma del token
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey))
            };
        });

        services.AddAuthorizationBuilder();

        return services;
    }
}
