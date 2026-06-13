namespace TattoStudio.Domain.Exceptions;

public class ArtistAlreadyInactiveException : Exception
{
    public ArtistAlreadyInactiveException(Guid id)
        : base($"Artist with id '{id}' is already inactive.") { }
}
