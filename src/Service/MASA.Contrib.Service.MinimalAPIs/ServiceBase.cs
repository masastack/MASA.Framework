using Microsoft.AspNetCore.Routing;

namespace MASA.Contrib.Service.MinimalAPIs;

public abstract class ServiceBase : IService
{
    private ServiceProvider _serviceProvider = default!;

    public WebApplication App => _serviceProvider.GetRequiredService<WebApplication>();

    public string BaseUri { get; }

    public IServiceCollection Services { get; protected set; }

    public ServiceBase(IServiceCollection services)
    {
        Services = services;
        _serviceProvider = services.BuildServiceProvider();
    }

    public ServiceBase(IServiceCollection services, string baseUri)
    {
        BaseUri = baseUri;
        Services = services;
        _serviceProvider = services.BuildServiceProvider();
    }

    public TService? GetService<TService>() => _serviceProvider.GetService<TService>();

    public TService GetRequiredService<TService>()
        where TService : notnull
        => Services.BuildServiceProvider().GetRequiredService<TService>();

    #region Map GET,POST,PUT,DELETE

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapGet(Delegate handler, string? customUri = null)
    {
        customUri ??= FormatAction(handler.Method.Name);

        return App.MapGet($"{BaseUri}/{customUri}", handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapPost(Delegate handler, string? customUri = null)
    {
        customUri ??= FormatAction(handler.Method.Name);

        return App.MapPost($"{BaseUri}/{customUri}", handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapPut(Delegate handler, string? customUri = null)
    {
        customUri ??= FormatAction(handler.Method.Name);

        return App.MapPut($"{BaseUri}/{customUri}", handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapDelete(Delegate handler, string? customUri = null)
    {
        customUri ??= FormatAction(handler.Method.Name);

        return App.MapDelete($"{BaseUri}/{customUri}", handler);
    }

    private static string FormatAction(string action)
    {
        if (!action.EndsWith("Async")) return action;

        var i = action.LastIndexOf("Async", StringComparison.Ordinal);
        return action[..i];
    }

    #endregion
}