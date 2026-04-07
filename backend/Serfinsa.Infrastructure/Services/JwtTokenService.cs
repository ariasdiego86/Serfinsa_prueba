using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serfinsa.Application.Interfaces;
using Serfinsa.Domain.Entities;

namespace Serfinsa.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de generación de tokens JWT.
/// Usa la clave secreta y configuración de appsettings.json (inyectada vía IConfiguration).
/// El token incluye claims de email, rol e identificador de usuario para la autorización basada en roles.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // Obtener configuración JWT de appsettings.json
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey no está configurada.");
        var issuer = jwtSettings["Issuer"] ?? "SerfinsaApi";
        var audience = jwtSettings["Audience"] ?? "SerfinsaClient";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        // Crear la clave de firma usando HMAC-SHA256
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims que se incluyen en el payload del token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // ClaimTypes.Role es el claim estándar que ASP.NET Core usa para [Authorize(Roles = "...")]
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
