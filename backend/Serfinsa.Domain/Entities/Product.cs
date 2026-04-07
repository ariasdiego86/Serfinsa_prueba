namespace Serfinsa.Domain.Entities;

/// <summary>
/// Entidad de dominio Producto.
/// Contiene la información de negocio de los productos gestionados en el sistema.
/// Cero dependencias externas — sigue el principio de Clean Architecture.
/// </summary>
public class Product
{
    public int Id { get; set; }

    /// <summary>Nombre del producto. Campo obligatorio.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción opcional del producto.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Precio del producto. Debe ser mayor a 0.</summary>
    public decimal Precio { get; set; }

    /// <summary>Cantidad en inventario. No puede ser negativo.</summary>
    public int Stock { get; set; }

    /// <summary>Categoría o tipo del producto (ej: "Electrónico", "Ropa").</summary>
    public string? TipoProducto { get; set; }
}
