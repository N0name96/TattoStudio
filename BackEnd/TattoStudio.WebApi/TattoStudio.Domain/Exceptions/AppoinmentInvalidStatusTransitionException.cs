using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Exceptions;

public class AppoinmentInvalidStatusTransitionException : Exception
{
    public AppoinmentInvalidStatusTransitionException(AppoinmentStatus from, AppoinmentStatus to)
        : base($"Cannot transition appoinment status from '{from}' to '{to}'.") { }
}
