using Serfinsa.Application.DTOs.Auth;

namespace Serfinsa.Application.Interfaces;

/// <summary>
/// Contrato del servicio de autenticación.
/// La interfaz en Application permite que Api la consuma sin conocer la implementación.
/// </summary>
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
