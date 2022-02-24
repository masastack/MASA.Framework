namespace Masa.Contrib.Service.MinimalAPIs;

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
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapGet(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatAction(handler.Method.Name, trimEndAsync);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapGet(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapPost(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatAction(handler.Method.Name, trimEndAsync);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapPost(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapPut(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatAction(handler.Method.Name, trimEndAsync);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapPut(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    protected RouteHandlerBuilder MapDelete(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatAction(handler.Method.Name, trimEndAsync);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapDelete(pattern, handler);
    }

    private static string FormatAction(string action, bool trimEndAsync)
    {
        if (trimEndAsync && action.EndsWith("Async"))
        {
            var i = action.LastIndexOf("Async", StringComparison.Ordinal);
            action = action[..i];
        }

        return action;
    }

    private static string CombineUris(params string[] uris)
    {
        return string.Join("/", uris.Select(u => u.Trim('/')));
    }

    #endregion
}
