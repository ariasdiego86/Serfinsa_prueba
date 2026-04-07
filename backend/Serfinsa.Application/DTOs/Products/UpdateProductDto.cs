namespace Serfinsa.Application.DTOs.Products;

/// <summary>
/// DTO de entrada para actualizar un Producto existente.
/// Es validado por UpdateProductValidator (FluentValidation) antes de procesarse.
/// </summary>
public class UpdateProductDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? TipoProducto { get; set; }
}
