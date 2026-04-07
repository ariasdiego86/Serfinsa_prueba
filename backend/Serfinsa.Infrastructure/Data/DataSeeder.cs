using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serfinsa.Application.Interfaces;
using Serfinsa.Domain.Entities;

namespace Serfinsa.Infrastructure.Data;

/// <summary>
/// Seeder de datos iniciales.
/// Se ejecuta al arrancar la aplicación y crea el usuario administrador por defecto
/// usando BCrypt.Net para generar el hash correcto en tiempo de ejecución.
/// Esto evita tener hashes hardcodeados en el SQL que podrían ser inválidos.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        // Aplicar migraciones pendientes automáticamente si las hay
        await context.Database.MigrateAsync();

        // Crear usuario admin solo si no existe
        if (!await context.Users.AnyAsync(u => u.Email == "admin@serfinsa.com"))
        {
            var admin = new User
            {
                Email = "admin@serfinsa.com",
                // Hash generado en tiempo de ejecución con BCrypt.Net (work factor 12)
                PasswordHash = passwordHasher.Hash("Admin123!"),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
            logger.LogInformation("Usuario administrador creado: admin@serfinsa.com");
        }

        // Insertar productos de ejemplo si la tabla está vacía
        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new Product { Nombre = "Laptop HP 15", Descripcion = "Intel Core i5, 8GB RAM, 256GB SSD", Precio = 799.99m, Stock = 15, TipoProducto = "Electrónico" },
                new Product { Nombre = "Mouse Inalámbrico Logitech", Descripcion = "Ergonómico, batería recargable", Precio = 35.50m, Stock = 50, TipoProducto = "Electrónico" },
                new Product { Nombre = "Teclado Mecánico Redragon", Descripcion = "RGB, switches Blue", Precio = 65.00m, Stock = 30, TipoProducto = "Electrónico" },
                new Product { Nombre = "Monitor Samsung 24\"", Descripcion = "Full HD, 75Hz, panel IPS", Precio = 220.00m, Stock = 10, TipoProducto = "Electrónico" },
                new Product { Nombre = "Silla Gamer DXRacer", Descripcion = "Ergonómica, soporte lumbar", Precio = 350.00m, Stock = 5, TipoProducto = "Mobiliario" }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("5 productos de ejemplo insertados.");
        }
    }
}
