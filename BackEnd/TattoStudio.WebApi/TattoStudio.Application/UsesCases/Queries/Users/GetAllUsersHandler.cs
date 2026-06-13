using MediatR;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Queries.Users;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDTO>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }
}
