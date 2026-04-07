using Microsoft.EntityFrameworkCore;
using Serfinsa.Domain.Entities;
using Serfinsa.Domain.Interfaces;
using Serfinsa.Infrastructure.Data;

namespace Serfinsa.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta del repositorio de productos usando EF Core.
/// Centraliza todas las operaciones de base de datos para la entidad Product.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FindAsync(id);

    public async Task AddAsync(Product product)
        => await _context.Products.AddAsync(product);

    public async Task UpdateAsync(Product product)
        // EF Core rastrea la entidad; simplemente marcamos el estado como modificado
        => _context.Products.Update(product);

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
            _context.Products.Remove(product);
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
