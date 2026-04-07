namespace Serfinsa.Domain.Exceptions;

/// <summary>
/// Excepción base para errores de lógica de dominio.
/// Al lanzar esta excepción, el GlobalExceptionHandler la convierte en una
/// respuesta HTTP 400 con ProblemDetails para un contrato de error consistente.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>
/// Se lanza cuando no se encuentra un recurso solicitado.
/// El GlobalExceptionHandler la mapea a HTTP 404.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string resource, object id)
        : base($"{resource} con id '{id}' no fue encontrado.") { }
}

/// <summary>
/// Se lanza cuando las credenciales de autenticación son inválidas.
/// El GlobalExceptionHandler la mapea a HTTP 401.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Credenciales inválidas.")
        : base(message) { }
}
