using TattoStudio.Domain.Enums;

namespace TattoStudio.Domain.Exceptions;

public class AppoinmentAlreadyFinalizedException : Exception
{
    public AppoinmentAlreadyFinalizedException(Guid id, AppoinmentStatus status)
        : base($"Appoinment '{id}' is already in a final state ('{status}') and cannot be changed.") { }
}
