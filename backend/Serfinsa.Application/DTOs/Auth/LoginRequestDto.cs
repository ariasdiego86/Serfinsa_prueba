namespace Serfinsa.Application.DTOs.Auth;

/// <summary>
/// DTO de entrada para el endpoint de Login.
/// Contiene solo los campos que el cliente debe enviar.
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
