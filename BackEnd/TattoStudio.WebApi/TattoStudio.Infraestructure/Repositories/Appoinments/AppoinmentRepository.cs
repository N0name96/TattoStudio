using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;

namespace TattoStudio.Infraestructure.Repositories.Appoinments
{
    public class AppoinmentRepository : IAppoinmentRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public AppoinmentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AppoinmentDTO> CreateAsync(CreateAppoinmentCommand request, CancellationToken cancellationToken)
        {
            bool conflict = await _context.Appoinments.AnyAsync(
                a => a.ArtistId == request.ArtistId && a.AppoinmentDate == request.AppoinmentDate,
                cancellationToken);

            if (conflict)
                throw new AppoinmentConflictException(
                    $"The artist already has an appointment on {request.AppoinmentDate:g}.");

            var entity = _mapper.Map<Appoinment>(request);

            entity.CreatedAt = DateTime.UtcNow;

            _context.Appoinments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppoinmentDTO>(entity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([id], cancellationToken);
            if (entity != null)
            {
                _context.Appoinments.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<AppoinmentDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AppoinmentDTO>>(entity);
        }

        public async Task<AppoinmentDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([id], cancellationToken)
                ?? throw new AppoinmentNotFoundException(id);
            return _mapper.Map<AppoinmentDTO>(entity);
        }

        public async Task<AppoinmentDTO> UpdateAsync(Guid AppoinmentID, UpdateAppoinmentRequest request, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([AppoinmentID], cancellationToken)
                ?? throw new AppoinmentNotFoundException(AppoinmentID);

            Guid targetArtist = request.ArtistId ?? entity.ArtistId;
            DateTime targetDate = request.AppoinmentDate ?? entity.AppoinmentDate;

            bool conflict = await _context.Appoinments.AnyAsync(
                a => a.Id != AppoinmentID && a.ArtistId == targetArtist && a.AppoinmentDate == targetDate,
                cancellationToken);

            if (conflict)
                throw new AppoinmentConflictException(
                    $"The artist already has an appointment on {targetDate:g}.");

            _mapper.Map(request, entity);

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppoinmentDTO>(entity);
        }
    }
}