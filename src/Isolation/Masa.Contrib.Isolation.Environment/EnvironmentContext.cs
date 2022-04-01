namespace Masa.Contrib.Isolation.Environment;

public class EnvironmentContext : IEnvironmentContext, IEnvironmentSetter
{
    public string CurrentEnvironment { get; private set; } = string.Empty;

    public void SetEnvironment(string environment) => CurrentEnvironment = environment;
}
