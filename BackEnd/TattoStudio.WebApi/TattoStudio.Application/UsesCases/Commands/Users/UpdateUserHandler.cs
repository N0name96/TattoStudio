using MediatR;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Users;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDTO>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.UpdateAsync(request.UserId, request.Data, cancellationToken);
    }
}
