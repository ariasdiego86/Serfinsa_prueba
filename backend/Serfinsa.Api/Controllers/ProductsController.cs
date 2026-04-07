using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serfinsa.Application.DTOs.Products;
using Serfinsa.Application.Interfaces;

namespace Serfinsa.Api.Controllers;

/// <summary>
/// Controlador de Productos (CRUD completo).
/// TODOS los endpoints requieren autenticación JWT ([Authorize]).
/// El rol "Admin" puede realizar todas las operaciones.
/// El rol "User" puede listar y consultar productos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere JWT válido en el header: Authorization: Bearer {token}
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Obtiene todos los productos.</summary>
    /// <response code="200">Lista de productos.</response>
    /// <response code="401">Token JWT no proporcionado o inválido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>Obtiene un producto por su ID.</summary>
    /// <response code="200">Producto encontrado.</response>
    /// <response code="404">Producto no encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    /// <summary>Crea un nuevo producto. Solo para el rol Admin.</summary>
    /// <response code="201">Producto creado exitosamente.</response>
    /// <response code="400">Datos inválidos.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")] // Solo administradores pueden crear productos
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new ValidationProblemDetails(validation.ToDictionary()));

        var created = await _productService.CreateAsync(dto);
        // Devolver 201 Created con la URI del nuevo recurso
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Actualiza un producto existente. Solo para el rol Admin.</summary>
    /// <response code="200">Producto actualizado.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">Producto no encontrado.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")] // Solo administradores pueden modificar productos
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new ValidationProblemDetails(validation.ToDictionary()));

        var updated = await _productService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>Elimina un producto. Solo para el rol Admin.</summary>
    /// <response code="204">Producto eliminado.</response>
    /// <response code="404">Producto no encontrado.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")] // Solo administradores pueden eliminar productos
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
