using Serfinsa.Domain.Entities;

namespace Serfinsa.Domain.Interfaces;

/// <summary>
/// Contrato del repositorio de productos.
/// Sigue el patrón Repository para desacoplar la lógica de acceso a datos
/// de los casos de uso (Application layer).
/// </summary>
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
