namespace Masa.Contrib.BasicAbility.Pm.Service;

public class EnvironmentService : IEnvironmentService
{
    private readonly ICallerProvider _callerProvider;

    public EnvironmentService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }
}
