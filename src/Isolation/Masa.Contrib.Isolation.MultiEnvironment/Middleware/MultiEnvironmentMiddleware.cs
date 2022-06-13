// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment.Middleware;

public class MultiEnvironmentMiddleware : IIsolationMiddleware
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MultiEnvironmentMiddleware>? _logger;
    private readonly IEnumerable<IParserProvider> _parserProviders;
    private readonly IEnvironmentContext _environmentContext;
    private readonly IEnvironmentSetter _environmentSetter;
    private readonly IMultiEnvironmentUserContext? _environmentUserContext;
    private readonly string _environmentKey;
    private bool _handled;

    public MultiEnvironmentMiddleware(IServiceProvider serviceProvider, string environmentKey,
        IEnumerable<IParserProvider>? parserProviders)
    {
        _serviceProvider = serviceProvider;
        _environmentKey = environmentKey;
        _parserProviders = parserProviders ?? GetDefaultParserProviders();
        _logger = _serviceProvider.GetService<ILogger<MultiEnvironmentMiddleware>>();
        _environmentContext = _serviceProvider.GetRequiredService<IEnvironmentContext>();
        _environmentSetter = _serviceProvider.GetRequiredService<IEnvironmentSetter>();
        _environmentUserContext = _serviceProvider.GetService<IMultiEnvironmentUserContext>();
    }

    public async Task HandleAsync()
    {
        if (_handled)
            return;

        if (!string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
        {
            _logger?.LogDebug($"The environment is successfully resolved, and the resolver is: empty");
            return;
        }

        if (_environmentUserContext is { IsAuthenticated: true, Environment: { } })
        {
            _environmentSetter.SetEnvironment(_environmentUserContext.Environment);
            return;
        }

        List<string> parsers = new();
        foreach (var environmentParserProvider in _parserProviders)
        {
            parsers.Add(environmentParserProvider.Name);
            if (await environmentParserProvider.ResolveAsync(_serviceProvider, _environmentKey,
                    environment => _environmentSetter.SetEnvironment(environment)))
            {
                _logger?.LogDebug("The environment is successfully resolved, and the resolver is: {Resolvers}", string.Join("、 ", parsers));
                _handled = true;
                return;
            }
        }
        _logger?.LogDebug("Failed to resolve environment, and the resolver is: {Resolvers}", string.Join("、 ", parsers));
        _handled = true;
    }

    private List<IParserProvider> GetDefaultParserProviders()
    {
        return new List<IParserProvider>
        {
            new HttpContextItemParserProvider(),
            new QueryStringParserProvider(),
            new FormParserProvider(),
            new RouteParserProvider(),
            new HeaderParserProvider(),
            new CookieParserProvider(),
            new EnvironmentVariablesParserProvider()
        };
    }
}
