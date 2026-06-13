using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;

namespace TattoStudio.Infraestructure.Repositories.StudioSettings;

public class StudioSettingsRepository : IStudioSettingsRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudioSettingsRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudioSettingsDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.StudioSettings.ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<StudioSettingsDTO>>(entities);
    }

    public async Task<StudioSettingsDTO?> GetFirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        var entity = await _context.StudioSettings.FirstOrDefaultAsync(cancellationToken);
        return entity is null ? null : _mapper.Map<StudioSettingsDTO>(entity);
    }

    public async Task<StudioSettingsDTO> CreateAsync(CreateStudioSettingsCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Domain.Entities.StudioSettings>(request);
        entity.Id = Guid.NewGuid();

        _context.StudioSettings.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StudioSettingsDTO>(entity);
    }

    public async Task<StudioSettingsDTO> UpdateAsync(Guid id, UpdateStudioSettingsRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.StudioSettings.FindAsync([id], cancellationToken)
            ?? throw new StudioSettingsNotFoundException(id);

        _mapper.Map(request, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StudioSettingsDTO>(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.StudioSettings.FindAsync([id], cancellationToken)
            ?? throw new StudioSettingsNotFoundException(id);

        _context.StudioSettings.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
