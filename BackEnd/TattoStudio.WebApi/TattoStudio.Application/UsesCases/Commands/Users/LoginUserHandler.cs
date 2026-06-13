using MediatR;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Users;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDTO>
{
    private readonly IUserRepository _userRepository;

    public LoginUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.LoginAsync(request, cancellationToken);
    }
}
