namespace Serfinsa.Application.DTOs.Auth;

/// <summary>
/// DTO de respuesta para el Login exitoso.
/// Devuelve el JWT y datos básicos del usuario autenticado.
/// </summary>
public class LoginResponseDto
{
    /// <summary>Token JWT firmado que el cliente debe guardar y enviar en cada petición.</summary>
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
