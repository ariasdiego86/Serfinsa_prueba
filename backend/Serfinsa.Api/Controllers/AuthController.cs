using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serfinsa.Application.DTOs.Auth;
using Serfinsa.Application.Interfaces;

namespace Serfinsa.Api.Controllers;

/// <summary>
/// Controlador de autenticación.
/// Expone el endpoint de Login que valida credenciales y devuelve un JWT.
/// Este endpoint es público (no requiere [Authorize]).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequestDto> _validator;

    public AuthController(IAuthService authService, IValidator<LoginRequestDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    /// <summary>
    /// Autentica al usuario y devuelve un JWT para acceder a los endpoints protegidos.
    /// </summary>
    /// <param name="request">Credenciales de acceso (email y contraseña).</param>
    /// <returns>JWT token y datos del usuario autenticado.</returns>
    /// <response code="200">Login exitoso. Devuelve el token JWT.</response>
    /// <response code="400">Datos de entrada inválidos (validación FluentValidation).</response>
    /// <response code="401">Credenciales incorrectas.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Validar el DTO con FluentValidation antes de pasar al servicio
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationProblemDetails(
                validationResult.ToDictionary()));

        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
