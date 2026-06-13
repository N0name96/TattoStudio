using MediatR;
using TattoStudio.Application.DTOs.Users;

namespace TattoStudio.Application.UsesCases.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDTO>;
