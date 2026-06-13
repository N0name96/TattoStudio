namespace TattoStudio.Domain.Exceptions;

public class UserEmailConflictException : Exception
{
    public UserEmailConflictException(string email)
        : base($"A user with email '{email}' already exists.") { }
}
