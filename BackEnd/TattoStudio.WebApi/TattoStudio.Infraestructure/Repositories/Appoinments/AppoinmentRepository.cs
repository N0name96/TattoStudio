using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;

namespace TattoStudio.Infraestructure.Repositories.Appoinments
{
    public class AppoinmentRepository : IAppoinmentRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        private static readonly HashSet<(AppoinmentStatus From, AppoinmentStatus To)> _validTransitions =
        [
            (AppoinmentStatus.Pending, AppoinmentStatus.Confirmed),
            (AppoinmentStatus.Pending, AppoinmentStatus.Cancelled),
            (AppoinmentStatus.Confirmed, AppoinmentStatus.Completed),
            (AppoinmentStatus.Confirmed, AppoinmentStatus.Cancelled),
        ];

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
            var entities = await _context.Appoinments.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AppoinmentDTO>>(entities);
        }

        public async Task<AppoinmentDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([id], cancellationToken)
                ?? throw new AppoinmentNotFoundException(id);
            return _mapper.Map<AppoinmentDTO>(entity);
        }

        public async Task<AppoinmentDTO> UpdateAsync(Guid appoinmentId, UpdateAppoinmentRequest request, Guid changedByUserId, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([appoinmentId], cancellationToken)
                ?? throw new AppoinmentNotFoundException(appoinmentId);

            Guid targetArtist = request.ArtistId ?? entity.ArtistId;
            DateTime targetDate = request.AppoinmentDate ?? entity.AppoinmentDate;

            bool conflict = await _context.Appoinments.AnyAsync(
                a => a.Id != appoinmentId && a.ArtistId == targetArtist && a.AppoinmentDate == targetDate,
                cancellationToken);

            if (conflict)
                throw new AppoinmentConflictException(
                    $"The artist already has an appointment on {targetDate:g}.");

            var auditLogs = new List<AppoinmentAuditLog>();
            var now = DateTime.UtcNow;

            if (request.AppoinmentDate.HasValue && request.AppoinmentDate.Value != entity.AppoinmentDate)
                auditLogs.Add(BuildAuditLog(appoinmentId, changedByUserId, "AppoinmentDate",
                    entity.AppoinmentDate.ToString("O"), request.AppoinmentDate.Value.ToString("O"), now));

            if (request.DurationMinutes.HasValue && request.DurationMinutes.Value != entity.DurationMinutes)
                auditLogs.Add(BuildAuditLog(appoinmentId, changedByUserId, "DurationMinutes",
                    entity.DurationMinutes.ToString(), request.DurationMinutes.Value.ToString(), now));

            if (request.ArtistId.HasValue && request.ArtistId.Value != entity.ArtistId)
                auditLogs.Add(BuildAuditLog(appoinmentId, changedByUserId, "ArtistId",
                    entity.ArtistId.ToString(), request.ArtistId.Value.ToString(), now));

            _mapper.Map(request, entity);
            entity.UpdatedAt = now;

            if (auditLogs.Count > 0)
                _context.AppoinmentAuditLogs.AddRange(auditLogs);

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppoinmentDTO>(entity);
        }

        public async Task<AppoinmentDTO> ChangeStatusAsync(Guid id, AppoinmentStatus newStatus, string? cancellationReason, Guid changedByUserId, CancellationToken cancellationToken)
        {
            var entity = await _context.Appoinments.FindAsync([id], cancellationToken)
                ?? throw new AppoinmentNotFoundException(id);

            if (entity.Status == AppoinmentStatus.Completed || entity.Status == AppoinmentStatus.Cancelled)
                throw new AppoinmentAlreadyFinalizedException(id, entity.Status);

            if (!_validTransitions.Contains((entity.Status, newStatus)))
                throw new AppoinmentInvalidStatusTransitionException(entity.Status, newStatus);

            var now = DateTime.UtcNow;
            var auditLogs = new List<AppoinmentAuditLog>
            {
                BuildAuditLog(id, changedByUserId, "Status", entity.Status.ToString(), newStatus.ToString(), now)
            };

            entity.Status = newStatus;
            entity.UpdatedAt = now;

            if (newStatus == AppoinmentStatus.Cancelled && cancellationReason is not null)
            {
                auditLogs.Add(BuildAuditLog(id, changedByUserId, "CancellationReason",
                    entity.CancellationReason, cancellationReason, now));
                entity.CancellationReason = cancellationReason;
            }

            _context.AppoinmentAuditLogs.AddRange(auditLogs);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppoinmentDTO>(entity);
        }

        public async Task<IEnumerable<AppoinmentAuditLogDTO>> GetAuditLogAsync(Guid appoinmentId, CancellationToken cancellationToken)
        {
            _ = await _context.Appoinments.FindAsync([appoinmentId], cancellationToken)
                ?? throw new AppoinmentNotFoundException(appoinmentId);

            var logs = await _context.AppoinmentAuditLogs
                .Where(l => l.AppoinmentId == appoinmentId)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AppoinmentAuditLogDTO>>(logs);
        }

        public async Task<IEnumerable<AppoinmentDTO>> GetByArtistAndDateAsync(Guid artistId, DateOnly date, CancellationToken cancellationToken)
        {
            var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);

            var entities = await _context.Appoinments
                .Where(a => a.ArtistId == artistId
                    && (a.Status == AppoinmentStatus.Pending || a.Status == AppoinmentStatus.Confirmed)
                    && a.AppoinmentDate >= start && a.AppoinmentDate < end)
                .OrderBy(a => a.AppoinmentDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AppoinmentDTO>>(entities);
        }

        private static AppoinmentAuditLog BuildAuditLog(Guid appoinmentId, Guid changedByUserId,
            string fieldName, string? oldValue, string newValue, DateTime changedAt) => new()
        {
            Id = Guid.NewGuid(),
            AppoinmentId = appoinmentId,
            ChangedByUserId = changedByUserId,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAt = changedAt
        };
    }
}
