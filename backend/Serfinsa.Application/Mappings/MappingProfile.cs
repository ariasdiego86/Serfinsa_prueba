using AutoMapper;
using Serfinsa.Application.DTOs.Products;
using Serfinsa.Domain.Entities;

namespace Serfinsa.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper que define las transformaciones entre entidades de dominio y DTOs.
/// Centraliza la configuración de mapeos para facilitar mantenimiento.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Lectura: Entidad -> DTO de respuesta
        CreateMap<Product, ProductDto>();

        // Creación: DTO de entrada -> nueva Entidad
        CreateMap<CreateProductDto, Product>();

        // Actualización: DTO de entrada -> Entidad existente (actualiza campos in-place)
        CreateMap<UpdateProductDto, Product>();
    }
}
