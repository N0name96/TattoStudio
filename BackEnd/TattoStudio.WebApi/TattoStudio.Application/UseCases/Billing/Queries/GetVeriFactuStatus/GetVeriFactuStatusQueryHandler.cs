using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.UseCases.Billing.Queries.GetVeriFactuStatus;

/// <summary>
/// Handler que devuelve el estado VeriFactu de la factura indicada.
/// </summary>
public sealed class GetVeriFactuStatusQueryHandler
    : IRequestHandler<GetVeriFactuStatusQuery, GetVeriFactuStatusResult>
{
    private readonly IInvoiceRepository _invoices;
    private readonly IMapper            _mapper;

    /// <summary>Inyecta el repositorio de facturas y el mapper.</summary>
    public GetVeriFactuStatusQueryHandler(IInvoiceRepository invoices, IMapper mapper)
    {
        _invoices = invoices;
        _mapper   = mapper;
    }

    /// <summary>Busca la factura por Id y devuelve su estado VeriFactu.</summary>
    public async Task<GetVeriFactuStatusResult> Handle(
        GetVeriFactuStatusQuery query,
        CancellationToken       ct)
    {
        var invoice = await _invoices.FindByIdAsync(query.InvoiceId, ct)
            ?? throw new NotFoundException($"Factura con Id '{query.InvoiceId}' no encontrada.");

        return _mapper.Map<GetVeriFactuStatusResult>(invoice);
    }
}
