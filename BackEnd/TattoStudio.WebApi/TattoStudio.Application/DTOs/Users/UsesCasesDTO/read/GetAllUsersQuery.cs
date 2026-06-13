using MediatR;
using TattoStudio.Application.DTOs.Users;

namespace TattoStudio.Application.UsesCases.Queries.Users;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDTO>>;
