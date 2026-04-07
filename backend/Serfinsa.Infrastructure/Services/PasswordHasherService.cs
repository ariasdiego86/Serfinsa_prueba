using Serfinsa.Application.Interfaces;

namespace Serfinsa.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de hashing de contraseñas usando BCrypt.
/// BCrypt incluye un salt automático y es resistente a ataques de fuerza bruta
/// gracias a su factor de trabajo configurable (work factor).
/// Se registra en DI como Scoped para evitar problemas de concurrencia.
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    // Factor de trabajo: determina cuántas iteraciones aplica BCrypt.
    // 12 es el valor recomendado como balance entre seguridad y rendimiento (2024+).
    private const int WorkFactor = 12;

    /// <summary>Genera un hash seguro con salt incluido.</summary>
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    /// <summary>Verifica si la contraseña en texto plano corresponde al hash almacenado.</summary>
    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
