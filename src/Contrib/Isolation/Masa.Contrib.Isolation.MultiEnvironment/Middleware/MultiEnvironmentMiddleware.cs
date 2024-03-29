// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment.Middleware;

public class MultiEnvironmentMiddleware : IIsolationMiddleware
{
    private const string DEFAULT_ENVIRONMENT_NAME = "ASPNETCORE_ENVIRONMENT";
    private readonly ILogger<MultiEnvironmentMiddleware>? _logger;
    private readonly IEnumerable<IParserProvider> _parserProviders;
    private readonly IMultiEnvironmentContext _environmentContext;
    private readonly IMultiEnvironmentSetter _environmentSetter;
    private readonly IMultiEnvironmentUserContext? _environmentUserContext;
    private readonly string _environmentKey;
    private bool _handled;

    public MultiEnvironmentMiddleware(
        IServiceProvider serviceProvider,
        string? environmentKey,
        IEnumerable<IParserProvider>? parserProviders,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null)
    {
        _environmentKey = environmentKey ??
            masaAppConfigureOptions?.Value.GetVariable(nameof(MasaAppConfigureOptions.Environment))?.Variable ??
            DEFAULT_ENVIRONMENT_NAME;
        _parserProviders = parserProviders ?? GetDefaultParserProviders();
        _logger = serviceProvider.GetService<ILogger<MultiEnvironmentMiddleware>>();
        _environmentContext = serviceProvider.GetRequiredService<IMultiEnvironmentContext>();
        _environmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        _environmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
    }

    public async Task HandleAsync(HttpContext? httpContext)
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
            if (await environmentParserProvider.ResolveAsync(httpContext, _environmentKey,
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
            new CurrentUserEnvironmentParseProvider(),
            new HttpContextItemParserProvider(),
            new QueryStringParserProvider(),
            new FormParserProvider(),
            new RouteParserProvider(),
            new HeaderParserProvider(),
            new CookieParserProvider(),
            new MasaAppConfigureParserProvider(),
            new EnvironmentVariablesParserProvider()
        };
    }
}
