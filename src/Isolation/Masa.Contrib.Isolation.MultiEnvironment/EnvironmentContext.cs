namespace Masa.Contrib.Isolation.MultiEnvironment;

public class EnvironmentContext : IEnvironmentContext, IEnvironmentSetter
{
    public string CurrentEnvironment { get; private set; } = string.Empty;

    public void SetEnvironment(string environment) => CurrentEnvironment = environment;
}
