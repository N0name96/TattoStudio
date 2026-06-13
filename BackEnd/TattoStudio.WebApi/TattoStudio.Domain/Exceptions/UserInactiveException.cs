namespace TattoStudio.Domain.Exceptions;

public class UserInactiveException : Exception
{
    public UserInactiveException()
        : base("User account is inactive.") { }
}
