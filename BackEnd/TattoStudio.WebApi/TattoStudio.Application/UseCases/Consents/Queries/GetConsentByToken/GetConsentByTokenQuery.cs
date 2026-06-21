using MediatR;

namespace TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;

/// <summary>
/// Query para obtener el estado del consentimiento vinculado al token de consentimiento.
/// Si el consentimiento no existe aún, se crea en esta llamada (lazy creation).
/// </summary>
/// <param name="Token">Token JWT de consentimiento generado por <c>GenerateQrQuery</c>.</param>
public record GetConsentByTokenQuery(string Token) : IRequest<ConsentDto>;
