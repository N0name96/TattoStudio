using MediatR;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Interfaces;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UsesCases.Commands.Appoinments;

public class ChangeAppoinmentStatusHandler : IRequestHandler<ChangeAppoinmentStatusCommand, AppoinmentDTO>
{
    private readonly IAppoinmentRepository _appoinmentRepository;
    private readonly IArtistRepository _artistRepository;

    public ChangeAppoinmentStatusHandler(IAppoinmentRepository appoinmentRepository, IArtistRepository artistRepository)
    {
        _appoinmentRepository = appoinmentRepository;
        _artistRepository = artistRepository;
    }

    public async Task<AppoinmentDTO> Handle(ChangeAppoinmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appoinment = await _appoinmentRepository.GetByIdAsync(request.AppoinmentId, cancellationToken);

        if (!request.IsAdmin)
        {
            var artist = await _artistRepository.GetByIdAsync(appoinment.ArtistId, cancellationToken);
            if (artist.UserId != request.RequestingUserId)
                throw new AppoinmentUnauthorizedStatusChangeException();
        }

        return await _appoinmentRepository.ChangeStatusAsync(
            request.AppoinmentId,
            request.NewStatus,
            request.CancellationReason,
            request.RequestingUserId,
            cancellationToken);
    }
}
