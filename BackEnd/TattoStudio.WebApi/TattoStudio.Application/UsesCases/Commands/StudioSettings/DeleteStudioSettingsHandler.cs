using MediatR;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;

namespace TattoStudio.Application.UsesCases.Commands.StudioSettings;

public class DeleteStudioSettingsHandler : IRequestHandler<DeleteStudioSettingsCommand, Unit>
{
    private readonly IStudioSettingsRepository _repository;

    public DeleteStudioSettingsHandler(IStudioSettingsRepository repository)
        => _repository = repository;

    public async Task<Unit> Handle(DeleteStudioSettingsCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
