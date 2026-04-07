using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serfinsa.Application.Mappings;
using Serfinsa.Application.Services;
using Serfinsa.Application.Interfaces;

namespace Serfinsa.Application;

/// <summary>
/// Extensión de IServiceCollection para registrar todos los servicios de la capa Application.
/// Centraliza el registro de DI siguiendo el patrón de extensiones por capa (Clean Architecture).
/// Se llama desde Program.cs en el proyecto Api.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar AutoMapper con el perfil de mapeos de esta capa
        services.AddAutoMapper(typeof(MappingProfile));

        // Registrar todos los validadores de FluentValidation del assembly de Application
        services.AddValidatorsFromAssemblyContaining<MappingProfile>();

        // Registrar servicios de aplicación (casos de uso)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
