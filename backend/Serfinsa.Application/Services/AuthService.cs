using Serfinsa.Application.DTOs.Auth;
using Serfinsa.Application.Interfaces;
using Serfinsa.Domain.Exceptions;
using Serfinsa.Domain.Interfaces;

namespace Serfinsa.Application.Services;

/// <summary>
/// Caso de uso: Autenticación de usuarios.
/// Verifica credenciales, valida el hash de contraseña y genera el JWT.
/// Depende únicamente de abstracciones (IUserRepository, IJwtTokenService, IPasswordHasherService)
/// siguiendo el principio de Inversión de Dependencias (SOLID).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Buscar usuario por email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Si el usuario no existe o la contraseña no coincide, lanzar excepción de dominio
        // Usamos el mismo mensaje para ambos casos para no revelar si el email existe (seguridad)
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Email o contraseña incorrectos.");

        // Generar JWT con el rol del usuario incluido como claim
        var token = _jwtTokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }
}
