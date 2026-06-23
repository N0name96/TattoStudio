using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;

namespace TattoStudio.Application.UseCases.Stock.Queries.GetLowInventory;

/// <summary>Handler de la query <see cref="GetLowInventoryQuery"/>. Devuelve ítems con stock bajo mapeados a DTO.</summary>
public sealed class GetLowInventoryQueryHandler : IRequestHandler<GetLowInventoryQuery, IReadOnlyList<StockItemDto>>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IMapper              _mapper;

    /// <summary>Inyecta el repositorio de stock y el mapper.</summary>
    public GetLowInventoryQueryHandler(IStockItemRepository stockItemRepository, IMapper mapper)
    {
        _stockItemRepository = stockItemRepository;
        _mapper              = mapper;
    }

    /// <summary>Recupera y mapea los ítems de bajo inventario.</summary>
    public async Task<IReadOnlyList<StockItemDto>> Handle(GetLowInventoryQuery query, CancellationToken ct)
    {
        var items = await _stockItemRepository.GetLowInventoryAsync(ct);
        return _mapper.Map<IReadOnlyList<StockItemDto>>(items);
    }
}
