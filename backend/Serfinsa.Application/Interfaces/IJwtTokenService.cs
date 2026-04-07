using Serfinsa.Domain.Entities;

namespace Serfinsa.Application.Interfaces;

/// <summary>
/// Contrato para la generación de tokens JWT.
/// La implementación vive en Infrastructure (accede a configuración y librerías externas).
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
}
