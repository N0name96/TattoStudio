namespace TattoStudio.Domain.Exceptions;

public class UserSelfDeleteException : Exception
{
    public UserSelfDeleteException()
        : base("An administrator cannot delete their own account.") { }
}
