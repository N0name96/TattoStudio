using AutoMapper;
using TattoStudio.Application.UseCases.Billing.Commands.IssueInvoice;
using TattoStudio.Application.UseCases.Billing.Queries.GetVeriFactuStatus;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="Invoice"/>.
/// Centraliza los mappings hacia resultados de comandos y consultas de facturación.
/// </summary>
public sealed class InvoiceProfile : Profile
{
    /// <summary>Registra todos los mapeos de la entidad Invoice.</summary>
    public InvoiceProfile()
    {
        CreateMap<Invoice, IssueInvoiceResult>()
            .ForCtorParam("AeataStatus", opt => opt.MapFrom(s => (int)s.AeataStatus));

        CreateMap<Invoice, GetVeriFactuStatusResult>()
            .ForCtorParam("AeataStatus", opt => opt.MapFrom(s => (int)s.AeataStatus));
    }
}
