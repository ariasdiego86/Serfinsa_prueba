using Serfinsa.Application.DTOs.Products;

namespace Serfinsa.Application.Interfaces;

/// <summary>
/// Contrato del servicio de productos (casos de uso CRUD).
/// Sigue el principio de Inversión de Dependencias (DIP): Api depende de esta abstracción.
/// </summary>
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}
