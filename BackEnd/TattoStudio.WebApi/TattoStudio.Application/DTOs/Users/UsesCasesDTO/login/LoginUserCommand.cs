using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TattoStudio.Application.DTOs.Users;

public record LoginUserCommand : IRequest<LoginResponseDTO>
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
