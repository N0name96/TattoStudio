using AutoMapper;
using MediatR;
using TattoStudio.Application.Contracts;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.UseCases.Stock.Commands.CreateStockItem;

/// <summary>Handler del comando <see cref="CreateStockItemCommand"/>. Crea y persiste un ítem de inventario.</summary>
public sealed class CreateStockItemCommandHandler : IRequestHandler<CreateStockItemCommand, CreateStockItemResult>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IMapper              _mapper;

    /// <summary>Inyecta el repositorio de stock y el mapper.</summary>
    public CreateStockItemCommandHandler(IStockItemRepository stockItemRepository, IMapper mapper)
    {
        _stockItemRepository = stockItemRepository;
        _mapper              = mapper;
    }

    /// <summary>Crea el ítem de stock y lo persiste en la base de datos.</summary>
    public async Task<CreateStockItemResult> Handle(CreateStockItemCommand command, CancellationToken ct)
    {
        var item = new StockItem(command.Name, command.CurrentQuantity, command.MinThreshold);

        await _stockItemRepository.AddAsync(item, ct);
        await _stockItemRepository.SaveChangesAsync(ct);

        return _mapper.Map<CreateStockItemResult>(item);
    }
}
