namespace TattoStudio.Domain.Exceptions;

public class AppoinmentUnauthorizedStatusChangeException : Exception
{
    public AppoinmentUnauthorizedStatusChangeException()
        : base("User is not authorized to change the status of this appoinment.") { }
}
