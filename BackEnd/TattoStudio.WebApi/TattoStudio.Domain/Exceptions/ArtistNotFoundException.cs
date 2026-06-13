namespace TattoStudio.Domain.Exceptions;

public class ArtistNotFoundException : Exception
{
    public ArtistNotFoundException(Guid id)
        : base($"Artist with id '{id}' was not found.") { }
}
