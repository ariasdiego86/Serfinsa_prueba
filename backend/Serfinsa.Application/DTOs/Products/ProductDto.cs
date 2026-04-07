namespace Serfinsa.Application.DTOs.Products;

/// <summary>
/// DTO de lectura de Producto. Se usa en las respuestas GET.
/// AutoMapper mapea desde la entidad Product a este DTO.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? TipoProducto { get; set; }
}
