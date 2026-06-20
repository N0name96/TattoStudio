using AutoMapper;
using TattoStudio.Application.UseCases.Auth.Commands.Register;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para el agregado de autenticación.
/// Centraliza todos los mappings de User hacia sus DTOs y resultados de comando.
/// </summary>
public sealed class AuthProfile : Profile
{
    public AuthProfile()
    {
        // User → RegisterUserResult
        // AutoMapper resuelve User.Id → RegisterUserResult.Id por convención de nombre.
        // Patrón establecido para futuros mappings de Query responses (UserDto, etc.)
        CreateMap<User, RegisterUserResult>();
    }
}
