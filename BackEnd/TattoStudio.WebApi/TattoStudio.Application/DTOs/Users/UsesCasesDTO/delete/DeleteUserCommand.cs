using MediatR;

namespace TattoStudio.Application.UsesCases.Commands.Users;

public record DeleteUserCommand(Guid UserId, Guid RequestingUserId) : IRequest<Unit>;
