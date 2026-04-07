namespace Serfinsa.Application.DTOs.Products;

/// <summary>
/// DTO de entrada para crear un nuevo Producto.
/// Es validado por CreateProductValidator (FluentValidation) antes de procesarse.
/// </summary>
public class CreateProductDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? TipoProducto { get; set; }
}
