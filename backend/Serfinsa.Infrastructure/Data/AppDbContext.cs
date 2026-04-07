using Microsoft.EntityFrameworkCore;
using Serfinsa.Domain.Entities;

namespace Serfinsa.Infrastructure.Data;

/// <summary>
/// DbContext de Entity Framework Core.
/// Configura las tablas Users y Products con sus restricciones mediante Fluent API.
/// La cadena de conexión se inyecta desde appsettings.json a través de IConfiguration.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            // Índice único para garantizar que no haya dos usuarios con el mismo email
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash)
                .IsRequired();
            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("User");
        });

        // Configuración de la entidad Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);
            // Precisión decimal para evitar errores de redondeo en precios
            entity.Property(p => p.Precio)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            entity.Property(p => p.Stock)
                .IsRequired();
            entity.Property(p => p.Descripcion)
                .HasMaxLength(1000);
            entity.Property(p => p.TipoProducto)
                .HasMaxLength(100);
        });
    }
}
