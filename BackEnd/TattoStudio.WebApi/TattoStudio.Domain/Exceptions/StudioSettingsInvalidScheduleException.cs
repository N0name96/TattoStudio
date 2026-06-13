namespace TattoStudio.Domain.Exceptions;

public class StudioSettingsInvalidScheduleException : Exception
{
    public StudioSettingsInvalidScheduleException()
        : base("WorkdayEnd must be later than WorkdayStart.") { }
}
