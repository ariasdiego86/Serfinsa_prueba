namespace Serfinsa.Domain.Entities;

/// <summary>
/// Entidad de dominio Usuario.
/// Representa un usuario del sistema con sus credenciales y rol.
/// Se mantiene en la capa de Dominio sin dependencias externas (Clean Architecture).
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Email único que sirve como identificador de acceso.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña almacenada como hash (BCrypt).
    /// Nunca se guarda la contraseña en texto plano.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario (ej: "Admin", "User").
    /// Se usa para la autorización basada en roles en los endpoints protegidos.
    /// </summary>
    public string Role { get; set; } = "User";
}
