using AutoMapper;
using Serfinsa.Application.DTOs.Products;
using Serfinsa.Application.Interfaces;
using Serfinsa.Domain.Entities;
using Serfinsa.Domain.Exceptions;
using Serfinsa.Domain.Interfaces;

namespace Serfinsa.Application.Services;

/// <summary>
/// Caso de uso: CRUD completo de Productos.
/// Orquesta el acceso a datos (IProductRepository) y el mapeo de objetos (AutoMapper).
/// Lanza excepciones de dominio que el GlobalExceptionHandler convierte en respuestas HTTP apropiadas.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        // AutoMapper convierte la colección de entidades a DTOs de forma eficiente
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // AutoMapper convierte el DTO de creación a entidad de dominio
        var product = _mapper.Map<Product>(dto);
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        // AutoMapper actualiza los campos de la entidad existente con los valores del DTO
        _mapper.Map(dto, product);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        await _productRepository.DeleteAsync(id);
        await _productRepository.SaveChangesAsync();
    }
}
