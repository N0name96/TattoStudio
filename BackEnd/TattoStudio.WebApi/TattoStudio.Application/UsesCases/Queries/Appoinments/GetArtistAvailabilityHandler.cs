using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Enums;

namespace TattoStudio.Application.UsesCases.Queries.Appoinments;

public class GetArtistAvailabilityHandler : IRequestHandler<GetArtistAvailabilityQuery, AvailabilityDTO>
{
    private readonly IArtistRepository _artistRepository;
    private readonly IAppoinmentRepository _appoinmentRepository;
    private readonly IStudioSettingsRepository _studioSettingsRepository;

    public GetArtistAvailabilityHandler(
        IArtistRepository artistRepository,
        IAppoinmentRepository appoinmentRepository,
        IStudioSettingsRepository studioSettingsRepository)
    {
        _artistRepository = artistRepository;
        _appoinmentRepository = appoinmentRepository;
        _studioSettingsRepository = studioSettingsRepository;
    }

    public async Task<AvailabilityDTO> Handle(GetArtistAvailabilityQuery request, CancellationToken cancellationToken)
    {
        await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);

        var appoinments = await _appoinmentRepository.GetByArtistAndDateAsync(request.ArtistId, request.Date, cancellationToken);
        var settings = await _studioSettingsRepository.GetFirstOrDefaultAsync(cancellationToken);

        var dayStart = settings is not null ? settings.WorkdayStart : new TimeOnly(0, 0);
        var dayEnd   = settings is not null ? settings.WorkdayEnd   : new TimeOnly(23, 59);

        var busyIntervals = appoinments
            .OrderBy(a => a.AppoinmentDate)
            .Select(a => (
                Start: TimeOnly.FromDateTime(a.AppoinmentDate),
                End:   TimeOnly.FromDateTime(a.AppoinmentDate).AddMinutes(a.DurationMinutes)))
            .ToList();

        var slots = new List<TimeSlotDTO>();
        var cursor = dayStart;

        foreach (var (start, end) in busyIntervals)
        {
            var clampedStart = start < dayStart ? dayStart : start;
            var clampedEnd   = end   > dayEnd   ? dayEnd   : end;

            if (cursor < clampedStart)
                slots.Add(new TimeSlotDTO(cursor, clampedStart));

            if (clampedEnd > cursor)
                cursor = clampedEnd;
        }

        if (cursor < dayEnd)
            slots.Add(new TimeSlotDTO(cursor, dayEnd));

        return new AvailabilityDTO
        {
            ArtistId       = request.ArtistId,
            Date           = request.Date,
            StudioStart    = settings?.WorkdayStart,
            StudioEnd      = settings?.WorkdayEnd,
            AvailableSlots = slots
        };
    }
}
