using Microsoft.EntityFrameworkCore;
using Serfinsa.Domain.Entities;
using Serfinsa.Domain.Interfaces;
using Serfinsa.Infrastructure.Data;

namespace Serfinsa.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta del repositorio de usuarios usando EF Core.
/// Vive en Infrastructure para no contaminar el Dominio con detalles de persistencia.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
