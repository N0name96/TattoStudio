using System.ComponentModel.DataAnnotations;
using MediatR;
using TattoStudio.Application.Validation;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.DTOs.Users;

public record RegisterUserCommand : IRequest<UserDTO>
{
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [PasswordStrength]
    public string Password { get; init; } = string.Empty;

    public UserRole Role { get; init; } = UserRole.User;
}
