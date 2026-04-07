namespace Serfinsa.Application.Interfaces;

/// <summary>
/// Contrato para el hash y verificación de contraseñas.
/// Desacopla Application de la librería de hashing (BCrypt en Infrastructure).
/// </summary>
public interface IPasswordHasherService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
