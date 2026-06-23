using AutoMapper;
using TattoStudio.Application.UseCases.Payments.Commands.RegisterPayment;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Payment"/>.
/// Centraliza los mappings hacia resultados de comandos de la Fase 5.
/// </summary>
public sealed class PaymentProfile : Profile
{
    /// <summary>Registra todos los mapeos de la entidad Payment.</summary>
    public PaymentProfile()
    {
        CreateMap<Payment, RegisterPaymentResult>();
    }
}
