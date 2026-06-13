using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UsesCases.Queries.Appoinments;

public class GetAppoinmentAuditLogHandler : IRequestHandler<GetAppoinmentAuditLogQuery, IEnumerable<AppoinmentAuditLogDTO>>
{
    private readonly IAppoinmentRepository _appoinmentRepository;
    private readonly IArtistRepository _artistRepository;

    public GetAppoinmentAuditLogHandler(IAppoinmentRepository appoinmentRepository, IArtistRepository artistRepository)
    {
        _appoinmentRepository = appoinmentRepository;
        _artistRepository = artistRepository;
    }

    public async Task<IEnumerable<AppoinmentAuditLogDTO>> Handle(GetAppoinmentAuditLogQuery request, CancellationToken cancellationToken)
    {
        var appoinment = await _appoinmentRepository.GetByIdAsync(request.AppoinmentId, cancellationToken);

        if (!request.IsAdmin)
        {
            var artist = await _artistRepository.GetByIdAsync(appoinment.ArtistId, cancellationToken);
            if (artist.UserId != request.RequestingUserId)
                throw new AppoinmentUnauthorizedStatusChangeException();
        }

        return await _appoinmentRepository.GetAuditLogAsync(request.AppoinmentId, cancellationToken);
    }
}
