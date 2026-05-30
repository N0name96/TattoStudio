namespace TattoStudio.Domain.Exceptions;

public class AppoinmentNotFoundException : Exception
{
    public AppoinmentNotFoundException(Guid id)
        : base($"Appoinment with id '{id}' was not found.") { }
}
