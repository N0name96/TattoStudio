using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;

namespace TattoStudio.Infraestructure.Repositories.Artists;

public class ArtistRepository : IArtistRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ArtistRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ArtistDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Artists.FindAsync([id], cancellationToken)
            ?? throw new ArtistNotFoundException(id);
        return _mapper.Map<ArtistDTO>(entity);
    }

    public async Task<IEnumerable<ArtistDTO>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Artists.AsQueryable();
        if (!includeInactive)
            query = query.Where(a => a.IsActive);
        var entities = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ArtistDTO>>(entities);
    }

    public async Task<ArtistDTO> CreateAsync(CreateArtistCommand request, CancellationToken cancellationToken)
    {
        bool mailExists = await _context.Artists.AnyAsync(
            a => a.Mail == request.Mail, cancellationToken);

        if (mailExists)
            throw new ArtistMailConflictException(request.Mail);

        var entity = _mapper.Map<Artist>(request);
        entity.Id = Guid.NewGuid();
        entity.IsActive = true;

        _context.Artists.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ArtistDTO>(entity);
    }

    public async Task<ArtistDTO> UpdateAsync(Guid artistId, UpdateArtistRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.Artists.FindAsync([artistId], cancellationToken)
            ?? throw new ArtistNotFoundException(artistId);

        string targetMail = request.Mail ?? entity.Mail;

        bool mailConflict = await _context.Artists.AnyAsync(
            a => a.Id != artistId && a.Mail == targetMail, cancellationToken);

        if (mailConflict)
            throw new ArtistMailConflictException(targetMail);

        _mapper.Map(request, entity);

        if (entity.IsActive && entity.DeactivatedAt.HasValue)
            entity.DeactivatedAt = null;
        else if (!entity.IsActive && !entity.DeactivatedAt.HasValue)
            entity.DeactivatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ArtistDTO>(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Artists.FindAsync([id], cancellationToken)
            ?? throw new ArtistNotFoundException(id);

        if (!entity.IsActive)
            throw new ArtistAlreadyInactiveException(id);

        entity.IsActive = false;
        entity.DeactivatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
