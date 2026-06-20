using MediatR;

namespace TattoStudio.Application.UseCases.Appointments.Commands.Create;

/// <summary>
/// Comando CQRS para reservar una nueva cita en el motor transaccional (Fase 3).
/// </summary>
public record CreateAppointmentCommand(
    Guid     ClientId,
    Guid     ArtistId,
    DateTime DateTime,
    int      DurationHours,
    decimal  DepositAmount) : IRequest<CreateAppointmentResult>;

/// <summary>Resultado del comando con los datos clave de la cita creada.</summary>
public record CreateAppointmentResult(Guid Id, bool HasDeposit);
