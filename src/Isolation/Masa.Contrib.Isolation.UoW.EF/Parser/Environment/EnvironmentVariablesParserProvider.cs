namespace Masa.Contrib.Isolation.UoW.EF.Parser.Environment;

public class EnvironmentVariablesParserProvider : IEnvironmentParserProvider
{
    public string Name { get; } = "EnvironmentVariables";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var environmentSetter = serviceProvider.GetRequiredService<IEnvironmentSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        string? environment = System.Environment.GetEnvironmentVariable(options.Value.EnvironmentKey);
        if (environment != null && !string.IsNullOrEmpty(environment))
        {
            environmentSetter.SetEnvironment(environment);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
