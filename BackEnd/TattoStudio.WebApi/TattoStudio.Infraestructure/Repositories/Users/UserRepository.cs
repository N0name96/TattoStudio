using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.DTOs.Users;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;

namespace TattoStudio.Infraestructure.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public UserRepository(AppDbContext context, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<UserDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.FindAsync([id], cancellationToken)
            ?? throw new UserNotFoundException(id);
        return _mapper.Map<UserDTO>(entity);
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.Users.ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserDTO>>(entities);
    }

    public async Task<UserDTO> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailLower = request.Email.ToLowerInvariant();

        bool emailExists = await _context.Users.AnyAsync(u => u.Email == emailLower, cancellationToken);
        if (emailExists)
            throw new UserEmailConflictException(request.Email);

        var entity = _mapper.Map<AppUser>(request);
        entity.Id = Guid.NewGuid();
        entity.Email = emailLower;
        entity.PasswordHash = _passwordHasher.Hash(request.Password);
        entity.IsActive = true;
        entity.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDTO>(entity);
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var emailLower = request.Email.ToLowerInvariant();
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailLower, cancellationToken);

        if (entity is null)
            throw new InvalidCredentialsException();

        if (!entity.IsActive)
            throw new UserInactiveException();

        if (!_passwordHasher.Verify(request.Password, entity.PasswordHash))
            throw new InvalidCredentialsException();

        var (token, expiresAt) = _jwtService.GenerateToken(entity.Id, entity.Email, entity.Role);

        return new LoginResponseDTO
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDTO> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.FindAsync([userId], cancellationToken)
            ?? throw new UserNotFoundException(userId);

        _mapper.Map(request, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDTO>(entity);
    }

    public async Task DeleteAsync(Guid userId, Guid requestingUserId, CancellationToken cancellationToken)
    {
        if (userId == requestingUserId)
            throw new UserSelfDeleteException();

        var entity = await _context.Users.FindAsync([userId], cancellationToken)
            ?? throw new UserNotFoundException(userId);

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
