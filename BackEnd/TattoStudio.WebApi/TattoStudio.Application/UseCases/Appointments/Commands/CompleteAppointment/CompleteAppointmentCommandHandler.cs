using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Appointments.Commands.CompleteAppointment;

/// <summary>
/// Handler del comando <see cref="CompleteAppointmentCommand"/>.
/// Implementa el flujo transaccional REG-05-01: verifica consentimiento, calcula comisión,
/// resta stock y marca la cita como Completada. Todo o nada.
/// </summary>
public sealed class CompleteAppointmentCommandHandler
    : IRequestHandler<CompleteAppointmentCommand, CompleteAppointmentResult>
{
    private readonly IAppointmentRepository          _appointmentRepository;
    private readonly IConsentRepository              _consentRepository;
    private readonly IPaymentRepository              _paymentRepository;
    private readonly IStockItemRepository            _stockItemRepository;
    private readonly IArtistCommissionsLogRepository _commissionsLogRepository;
    private readonly IUnitOfWork                     _uow;

    /// <summary>Inyecta todos los repositorios y la unidad de trabajo.</summary>
    public CompleteAppointmentCommandHandler(
        IAppointmentRepository          appointmentRepository,
        IConsentRepository              consentRepository,
        IPaymentRepository              paymentRepository,
        IStockItemRepository            stockItemRepository,
        IArtistCommissionsLogRepository commissionsLogRepository,
        IUnitOfWork                     uow)
    {
        _appointmentRepository    = appointmentRepository;
        _consentRepository        = consentRepository;
        _paymentRepository        = paymentRepository;
        _stockItemRepository      = stockItemRepository;
        _commissionsLogRepository = commissionsLogRepository;
        _uow                      = uow;
    }

    /// <summary>Ejecuta el cierre transaccional de la cita.</summary>
    public async Task<CompleteAppointmentResult> Handle(CompleteAppointmentCommand command, CancellationToken ct)
    {
        // 1. Obtener la cita con navegación al artista
        var appointment = await _appointmentRepository.FindByIdWithArtistAsync(command.AppointmentId, ct)
            ?? throw new NotFoundException($"Cita '{command.AppointmentId}' no encontrada.");

        var artist = appointment.Artist!;

        // 2. Calcular el total de pagos y la comisión
        var payments    = await _paymentRepository.GetByAppointmentIdAsync(appointment.Id, ct);
        var totalAmount = payments.Sum(p => p.Amount);
        var commissionAmount = totalAmount * (artist.CommissionPercentage / 100m);

        // 3. Verificar que el consentimiento existe y está firmado
        var consent = await _consentRepository.GetByAppointmentIdAsync(appointment.Id, ct);
        if (consent is null || !consent.IsSigned)
            throw new BusinessException("El consentimiento de la cita no está firmado.");

        // 4. Iniciar la transacción
        await _uow.BeginTransactionAsync(ct);

        // 5. Registrar la comisión
        var commissionLog = new ArtistCommissionsLog(artist.Id, appointment.Id, commissionAmount);
        await _commissionsLogRepository.AddAsync(commissionLog, ct);

        // 6. Descontar stock y calcular requiresRestock
        var requiresRestock = false;
        foreach (var stockUsage in command.StockItems)
        {
            var stockItem = await _stockItemRepository.FindByNameAsync(stockUsage.Name, ct);
            if (stockItem is not null)
            {
                stockItem.Consume(stockUsage.Quantity);
                requiresRestock |= stockItem.RequiresRestock;
            }
        }

        // 7. Marcar la cita como completada
        appointment.Complete();

        // 8. Confirmar la transacción (incluye SaveChanges)
        await _uow.CommitAsync(ct);

        return new CompleteAppointmentResult(
            appointment.Id,
            "Completada",
            commissionAmount,
            requiresRestock);
    }
}
