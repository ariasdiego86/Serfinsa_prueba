using AutoMapper;
using FluentAssertions;
using Moq;
using Serfinsa.Application.DTOs.Products;
using Serfinsa.Application.Mappings;
using Serfinsa.Application.Services;
using Serfinsa.Domain.Entities;
using Serfinsa.Domain.Exceptions;
using Serfinsa.Domain.Interfaces;
using Xunit;

namespace Serfinsa.Tests.Products;

/// <summary>
/// Pruebas unitarias del servicio de productos (ProductService).
/// Usa Moq para el repositorio y una instancia real de AutoMapper con MappingProfile.
/// Esto valida tanto la lógica de negocio como la correcta configuración de los mapeos.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();

        // Usar el MappingProfile real para validar la configuración de AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _productService = new ProductService(_repositoryMock.Object, _mapper);
    }

    [Fact(DisplayName = "GetAll: devuelve todos los productos como DTOs")]
    public async Task GetAll_DevuelveTodosLosProductosComoDto()
    {
        // Arrange
        var productos = new List<Product>
        {
            new() { Id = 1, Nombre = "Laptop", Precio = 999.99m, Stock = 10 },
            new() { Id = 2, Nombre = "Mouse", Precio = 25.50m, Stock = 50 }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(productos);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].Nombre.Should().Be("Laptop");
        resultList[1].Precio.Should().Be(25.50m);
    }

    [Fact(DisplayName = "GetById: devuelve el producto correcto cuando existe")]
    public async Task GetById_ProductoExistente_DevuelveDto()
    {
        // Arrange
        var producto = new Product
        {
            Id = 1,
            Nombre = "Teclado",
            Precio = 75.00m,
            Stock = 20,
            TipoProducto = "Electrónico"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(producto);

        // Act
        var result = await _productService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Nombre.Should().Be("Teclado");
        result.TipoProducto.Should().Be("Electrónico");
    }

    [Fact(DisplayName = "GetById: lanza NotFoundException cuando el producto no existe")]
    public async Task GetById_ProductoNoExistente_LanzaNotFoundException()
    {
        // Arrange: el repositorio devuelve null para el id solicitado
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.GetByIdAsync(999));
    }

    [Fact(DisplayName = "Create: agrega el producto y devuelve el DTO creado")]
    public async Task Create_ProductoValido_DevuelveDtoCreado()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Nombre = "Monitor",
            Precio = 350.00m,
            Stock = 5,
            TipoProducto = "Electrónico"
        };

        // El repositorio simula el guardado (id asignado después de SaveChanges)
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask)
            .Callback<Product>(p => p.Id = 1); // Simula que EF asignó el Id

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.CreateAsync(dto);

        // Assert
        result.Nombre.Should().Be("Monitor");
        result.Precio.Should().Be(350.00m);

        // Verificar que el repositorio fue llamado con AddAsync y SaveChangesAsync
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact(DisplayName = "Delete: lanza NotFoundException al intentar eliminar un producto inexistente")]
    public async Task Delete_ProductoNoExistente_LanzaNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.DeleteAsync(99));

        // Verificar que nunca se intentó eliminar ni guardar
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(DisplayName = "Update: lanza NotFoundException al actualizar un producto inexistente")]
    public async Task Update_ProductoNoExistente_LanzaNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        var dto = new UpdateProductDto { Nombre = "X", Precio = 10, Stock = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.UpdateAsync(99, dto));
    }
}
