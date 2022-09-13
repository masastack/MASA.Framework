// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder;

public abstract class ServiceBase : IService
{
    private readonly ServiceProvider _serviceProvider;

    public WebApplication App => _serviceProvider.GetRequiredService<WebApplication>();

    public string? BaseUri { get; init; }

    public Url Url { get; init; } = new();

    public bool DisableRestful { get; init; } = MasaService.DisableRestful;

    public IServiceCollection Services { get; protected set; }

    protected ServiceBase(IServiceCollection services)
    {
        Services = services;
        _serviceProvider = services.BuildServiceProvider();
    }

    protected ServiceBase(IServiceCollection services, string baseUri) : this(services)
    {
        BaseUri = baseUri;
    }

    public TService? GetService<TService>() => GetServiceProvider().GetService<TService>();

    public TService GetRequiredService<TService>() where TService : notnull
        => GetServiceProvider().GetRequiredService<TService>();

    protected virtual IServiceProvider GetServiceProvider()
        => MasaApp.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices ?? MasaApp.RootServiceProvider;

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
        customUri ??= FormatMethodName(handler.Method.Name, trimEndAsync);

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
        customUri ??= FormatMethodName(handler.Method.Name, trimEndAsync);

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
        customUri ??= FormatMethodName(handler.Method.Name, trimEndAsync);

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
        customUri ??= FormatMethodName(handler.Method.Name, trimEndAsync);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapDelete(pattern, handler);
    }

    public static string FormatMethodName(string methodName, bool trimEndAsync)
    {
        if (trimEndAsync && methodName.EndsWith("Async"))
        {
            var i = methodName.LastIndexOf("Async", StringComparison.Ordinal);
            methodName = methodName[..i];
        }

        return methodName;
    }

    public static string CombineUris(params string[] uris)
        => string.Join("/", uris.Select(u => u.Trim('/')));

    #endregion

    protected virtual string GetBaseUri()
    {
        if (DisableRestful) return string.Empty;

        return string.IsNullOrWhiteSpace(BaseUri) ? Url.ToString(GetType()) : BaseUri;
    }

    internal void AutoMapRouter()
    {
        var type = GetType();

        var methods = type
            .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            .Where(methodInfo => methodInfo.CustomAttributes.All(attr => attr.AttributeType != typeof(ExcludeMappingAttribute)))
            .Concat(type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(methodInfo => methodInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(IncludeMappingAttribute))));

        foreach (var method in methods)
        {
            var @delegate = CreateDelegate(method, this);

            var attribute = method.GetCustomAttributes(typeof(IncludeMappingAttribute), true).FirstOrDefault();
            string? pattern = null;
            string? httpMethod = null;
            if (attribute is not null && attribute is IncludeMappingAttribute includeMapping)
            {
                pattern = includeMapping.Pattern;
                httpMethod = includeMapping.Method?.ToUpper();
            }

            pattern ??= CombineUris(GetBaseUri(), FormatMethodName(method.Name, true));
            httpMethod ??= GetHttpMethod(method.Name);

            if (httpMethod != null) MapMethods(@delegate, pattern, httpMethod);
        }
    }

    protected virtual string? GetHttpMethod(string methodName)
    {
        if (methodName.StartsWith("Get", StringComparison.OrdinalIgnoreCase)) return "GET";

        if (methodName.StartsWith("Add", StringComparison.OrdinalIgnoreCase) ||
            methodName.StartsWith("Upsert", StringComparison.OrdinalIgnoreCase)) return "POST";

        if (methodName.StartsWith("Update", StringComparison.OrdinalIgnoreCase)) return "PUT";

        if (methodName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase) ||
            methodName.StartsWith("Delete", StringComparison.OrdinalIgnoreCase)) return "DELETE";

        Logger?.LogDebug("Failed to get HttpMethod");

        return null;
    }

    protected virtual ILogger<ServiceBase>? Logger => _serviceProvider.GetService<ILogger<ServiceBase>>();

    private RouteHandlerBuilder MapMethods(Delegate handler, string pattern, string httpMethod)
    {
        if (httpMethod == "GET")
        {
            return App.MapGet(pattern, handler);
        }
        return App.MapMethods(pattern, new[] { httpMethod }, handler);
    }

    private static Delegate CreateDelegate(MethodInfo methodInfo, object targetInstance)
    {
        var type = Expression.GetDelegateType(methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType)
            .Concat(new List<Type>
                { methodInfo.ReturnType }).ToArray());
        return Delegate.CreateDelegate(type, targetInstance, methodInfo);
    }
}
