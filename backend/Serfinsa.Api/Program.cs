using Microsoft.OpenApi.Models;
using Serfinsa.Api.Middleware;
using Serfinsa.Application;
using Serfinsa.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Registro de servicios por capa (Clean Architecture)
// =========================================================

// Capa Application: servicios de negocio, AutoMapper, FluentValidation
builder.Services.AddApplication();

// Capa Infrastructure: EF Core, repositorios, JWT, hashing
builder.Services.AddInfrastructure(builder.Configuration);

// Controladores con soporte para ActionResult tipados
builder.Services.AddControllers();

// Manejador global de excepciones (IExceptionHandler de ASP.NET Core 8+)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// =========================================================
// Configuración de Swagger/OpenAPI con soporte para JWT
// CRÍTICO: Permite probar endpoints protegidos directamente
//          desde la UI de Swagger usando el token Bearer.
// =========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Serfinsa API",
        Version = "v1",
        Description = "API de gestión de productos con autenticación JWT. " +
                      "Haz login en /api/auth/login, copia el token y usa el botón 'Authorize'."
    });

    // Definir el esquema de seguridad JWT Bearer para Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT en el formato: Bearer {tu_token}\n\n" +
                      "Ejemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    // Aplicar el esquema de seguridad globalmente a todos los endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir comentarios XML de los controladores en Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Configuración CORS para permitir peticiones desde el frontend React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Puerto por defecto de Vite
                "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// =========================================================
// Pipeline de middlewares (orden es importante)
// =========================================================

// El ExceptionHandler debe estar primero para capturar errores en todo el pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serfinsa API v1");
        // Configurar Swagger UI para persistir el token entre recargas de página
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.UseHttpsRedirection();

// CORS debe ir antes de Authentication y Authorization
app.UseCors("AllowFrontend");

// Autenticación JWT: verifica el token en el header Authorization
app.UseAuthentication();

// Autorización: verifica los roles del token para [Authorize(Roles = "...")]
app.UseAuthorization();

app.MapControllers();

app.Run();

// Clase parcial para exponer en las pruebas de integración (xUnit)
public partial class Program { }
