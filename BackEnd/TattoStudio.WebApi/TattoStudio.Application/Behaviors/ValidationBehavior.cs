using FluentValidation;
using MediatR;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.Application.Behaviors;

/// <summary>
/// Pipeline behavior de MediatR que ejecuta todos los validadores FluentValidation
/// registrados para el comando antes de que llegue al handler.
/// Lanza <see cref="BusinessException"/> con el primer error encontrado (HTTP 400).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest                          request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context  = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new BusinessException(failures[0].ErrorMessage);

        return await next();
    }
}
