namespace TattoStudio.Domain.Exceptions;

public class AppoinmentConflictException : Exception
{
    public AppoinmentConflictException(string message) : base(message) { }
}
