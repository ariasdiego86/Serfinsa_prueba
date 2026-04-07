using FluentAssertions;
using Moq;
using Serfinsa.Application.DTOs.Auth;
using Serfinsa.Application.Interfaces;
using Serfinsa.Application.Services;
using Serfinsa.Domain.Entities;
using Serfinsa.Domain.Exceptions;
using Serfinsa.Domain.Interfaces;
using Xunit;

namespace Serfinsa.Tests.Auth;

/// <summary>
/// Pruebas unitarias del servicio de autenticación (AuthService).
/// Usa Moq para simular las dependencias (repositorios y servicios externos)
/// y aislar la lógica del caso de uso bajo prueba.
/// </summary>
public class AuthServiceTests
{
    // Mocks de las dependencias del AuthService
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IPasswordHasherService> _passwordHasherMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _passwordHasherMock = new Mock<IPasswordHasherService>();

        // Crear la instancia bajo prueba con las dependencias mockeadas
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact(DisplayName = "Login exitoso: devuelve token cuando las credenciales son correctas")]
    public async Task Login_CredencialesCorrectas_DevuelveToken()
    {
        // Arrange: preparar datos de prueba
        var email = "admin@serfinsa.com";
        var password = "Admin123!";
        var hashedPassword = "hashed_password_bcrypt";
        var expectedToken = "jwt_token_generado";

        var usuarioExistente = new User
        {
            Id = 1,
            Email = email,
            PasswordHash = hashedPassword,
            Role = "Admin"
        };

        // Configurar mocks para simular comportamiento esperado
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(usuarioExistente);

        _passwordHasherMock
            .Setup(h => h.Verify(password, hashedPassword))
            .Returns(true); // Contraseña correcta

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(usuarioExistente))
            .Returns(expectedToken);

        var request = new LoginRequestDto { Email = email, Password = password };

        // Act: ejecutar el caso de uso
        var result = await _authService.LoginAsync(request);

        // Assert: verificar el resultado
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Email.Should().Be(email);
        result.Role.Should().Be("Admin");
    }

    [Fact(DisplayName = "Login falla: lanza UnauthorizedException cuando el usuario no existe")]
    public async Task Login_UsuarioNoExiste_LanzaUnauthorizedException()
    {
        // Arrange: el repositorio no encuentra el usuario
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequestDto
        {
            Email = "noexiste@serfinsa.com",
            Password = "cualquiera"
        };

        // Act & Assert: se espera la excepción de dominio UnauthorizedException
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.LoginAsync(request));
    }

    [Fact(DisplayName = "Login falla: lanza UnauthorizedException cuando la contraseña es incorrecta")]
    public async Task Login_ContrasenaIncorrecta_LanzaUnauthorizedException()
    {
        // Arrange
        var usuario = new User
        {
            Id = 1,
            Email = "admin@serfinsa.com",
            PasswordHash = "hash_correcto",
            Role = "Admin"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("admin@serfinsa.com"))
            .ReturnsAsync(usuario);

        _passwordHasherMock
            .Setup(h => h.Verify("contraseña_incorrecta", "hash_correcto"))
            .Returns(false); // Contraseña incorrecta

        var request = new LoginRequestDto
        {
            Email = "admin@serfinsa.com",
            Password = "contraseña_incorrecta"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.LoginAsync(request));
    }

    [Fact(DisplayName = "Login llama exactamente una vez al repositorio de usuarios")]
    public async Task Login_SiempreLlamaAlRepositorioDeUsuarios()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "password"
        };

        // Act
        try { await _authService.LoginAsync(request); } catch { }

        // Assert: verificar que el repositorio fue consultado exactamente una vez
        _userRepositoryMock.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
    }
}
