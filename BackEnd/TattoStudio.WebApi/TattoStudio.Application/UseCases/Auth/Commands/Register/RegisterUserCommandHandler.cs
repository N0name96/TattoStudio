using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Auth.Commands.Register;

/// <summary>
/// Handler que valida unicidad de email, hashea la contraseña y persiste el nuevo usuario.
/// </summary>
public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IMapper         _mapper;

    public RegisterUserCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IMapper         mapper)
    {
        _users  = users;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(cmd.Email, ct))
            throw new BusinessException("Email ya registrado");

        // new User() es necesario aquí: Hash() es un efecto secundario que AutoMapper
        // no puede resolver sin un IValueResolver. El mapeo puro va en el return.
        var user = new User(
            email: cmd.Email,
            passwordHash: _hasher.Hash(cmd.Password),
            name: cmd.Name,
            role: (Role)cmd.Role
        );

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return _mapper.Map<RegisterUserResult>(user);
    }
}
