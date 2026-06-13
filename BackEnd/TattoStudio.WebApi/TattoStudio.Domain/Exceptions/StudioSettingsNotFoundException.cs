namespace TattoStudio.Domain.Exceptions;

public class StudioSettingsNotFoundException : Exception
{
    public StudioSettingsNotFoundException(Guid id)
        : base($"StudioSettings with id '{id}' was not found.") { }
}
