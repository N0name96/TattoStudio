using MediatR;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Users;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDTO>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.RegisterAsync(request, cancellationToken);
    }
}
