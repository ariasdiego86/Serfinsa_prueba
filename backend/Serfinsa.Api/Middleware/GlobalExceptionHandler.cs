using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serfinsa.Domain.Exceptions;
using System.Net;

namespace Serfinsa.Api.Middleware;

/// <summary>
/// Manejador global de excepciones usando IExceptionHandler de ASP.NET Core 8+.
/// Captura excepciones no manejadas y las convierte en respuestas ProblemDetails (RFC 7807)
/// para un contrato de error consistente y manejable en el frontend.
/// Se registra en Program.cs con app.UseExceptionHandler().
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Determinar el código de estado HTTP según el tipo de excepción de dominio
        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "No autorizado"),
            DomainException => (HttpStatusCode.BadRequest, "Error de dominio"),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        // Loguear errores inesperados para trazabilidad
        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Error no manejado: {Message}", exception.Message);

        // Construir respuesta ProblemDetails (estándar RFC 7807)
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornar true indica que la excepción fue manejada y no debe propagarse
        return true;
    }
}
