using MediatR;

namespace TattoStudio.Application.DTOs.Users;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Data) : IRequest<UserDTO>;
