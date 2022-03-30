namespace Masa.Contrib.Isolation.UoW.EF.Middleware;

public class EnvironmentMiddleware : IIsolationMiddleware
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EnvironmentMiddleware>? _logger;
    private readonly IEnvironmentContext _environmentContext;
    private readonly IEnumerable<IEnvironmentParserProvider> _environmentParserProviders;
    private bool _handled;

    public EnvironmentMiddleware(IServiceProvider serviceProvider, IEnumerable<IEnvironmentParserProvider> environmentParserProviders)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<ILogger<EnvironmentMiddleware>>();
        _environmentContext = _serviceProvider.GetRequiredService<IEnvironmentContext>();
        _environmentParserProviders = environmentParserProviders;
    }

    public async Task HandleAsync()
    {
        if(_handled)
            return;

        if (!string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
        {
            _logger?.LogDebug($"The environment is successfully resolved, and the resolver is: empty");
            return;
        }

        List<string> parsers = new();
        foreach (var environmentParserProvider in _environmentParserProviders)
        {
            parsers.Add(environmentParserProvider.Name);
            if (await environmentParserProvider.ResolveAsync(_serviceProvider))
            {
                _logger?.LogDebug($"The environment is successfully resolved, and the resolver is: {string.Join("、 ",parsers)}");
                _handled = true;
                return;
            }
        }
        _logger?.LogDebug($"Failed to resolve environment, and the resolver is: {string.Join("、 ",parsers)}");
        _handled = true;
    }
}
