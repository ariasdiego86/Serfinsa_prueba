using FluentValidation;
using Serfinsa.Application.DTOs.Auth;

namespace Serfinsa.Application.Validators;

/// <summary>
/// Validación declarativa del request de Login usando FluentValidation.
/// Se registra en DI y se ejecuta antes de llegar al caso de uso.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
