namespace TattoStudio.Domain.Exceptions;

public class ArtistMailConflictException : Exception
{
    public ArtistMailConflictException(string mail)
        : base($"An artist with mail '{mail}' already exists.") { }
}
