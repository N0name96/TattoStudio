using MediatR;
using TattoStudio.Application.UseCases.Consents.Queries.GetConsentByToken;

namespace TattoStudio.Application.UseCases.Consents.Commands.SignConsent;

/// <summary>
/// Comando para firmar el consentimiento vinculado a un token JWT de consentimiento.
/// La firma es inmutable: si el consentimiento ya fue firmado se lanza un error 403 (REG-04-02).
/// </summary>
/// <param name="Token">Token JWT de consentimiento que identifica la cita.</param>
/// <param name="SignatureBase64">Firma del cliente codificada en Base64.</param>
public record SignConsentCommand(string Token, string SignatureBase64) : IRequest<ConsentDto>;
