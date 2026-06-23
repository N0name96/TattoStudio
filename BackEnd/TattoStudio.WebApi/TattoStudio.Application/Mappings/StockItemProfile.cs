using AutoMapper;
using TattoStudio.Application.UseCases.Stock.Commands.CreateStockItem;
using TattoStudio.Application.UseCases.Stock.Queries.GetLowInventory;
using TattoStudio.Domain.Entities;

namespace TattoStudio.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para la entidad <see cref="StockItem"/>.
/// Centraliza los mappings hacia resultados de comandos y queries de la Fase 5.
/// </summary>
public sealed class StockItemProfile : Profile
{
    /// <summary>Registra todos los mapeos de la entidad StockItem.</summary>
    public StockItemProfile()
    {
        CreateMap<StockItem, StockItemDto>()
            .ForMember(d => d.RequiresRestock, opt => opt.MapFrom(s => s.CurrentQuantity <= s.MinThreshold));

        CreateMap<StockItem, CreateStockItemResult>();
    }
}
