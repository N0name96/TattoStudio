using FluentValidation;

namespace TattoStudio.Application.UseCases.Stock.Commands.CreateStockItem;

/// <summary>Validador FluentValidation para <see cref="CreateStockItemCommand"/>.</summary>
public class CreateStockItemCommandValidator : AbstractValidator<CreateStockItemCommand>
{
    /// <summary>Define las reglas de validación del comando.</summary>
    public CreateStockItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CurrentQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinThreshold).GreaterThanOrEqualTo(0);
    }
}
