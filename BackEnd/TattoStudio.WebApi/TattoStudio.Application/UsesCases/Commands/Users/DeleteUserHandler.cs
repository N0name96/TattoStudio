using MediatR;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.Users;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(request.UserId, request.RequestingUserId, cancellationToken);
        return Unit.Value;
    }
}
