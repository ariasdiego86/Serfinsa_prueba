using Serfinsa.Domain.Entities;

namespace Serfinsa.Domain.Interfaces;

/// <summary>
/// Contrato del repositorio de usuarios.
/// La interfaz vive en Domain para invertir la dependencia (DIP - SOLID):
/// Infrastructure implementa, Application consume.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
