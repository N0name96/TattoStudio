using MediatR;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Users;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDTO>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
