// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public abstract class ServiceBase : IService
{
    public WebApplication App => MasaApp.GetRequiredService<WebApplication>();

    public string BaseUri { get; init; }

    public ServiceRouteOptions RouteOptions { get; } = new();

    public string? ServiceName { get; init; }

    /// <summary>
    /// Based on the RouteHandlerBuilder extension, it is used to extend the mapping method, such as
    /// RouteHandlerBuilder = routeHandlerBuilder =>
    /// {
    ///     routeHandlerBuilder.RequireAuthorization("AtLeast21");
    /// };
    /// </summary>
    public Action<RouteHandlerBuilder>? RouteHandlerBuilder { get; init; }

    public IServiceCollection Services => MasaApp.Services;

#pragma warning disable S4136
    protected ServiceBase() { }

    protected ServiceBase(string baseUri)
    {
        BaseUri = baseUri;
    }
#pragma warning restore S4136

    public TService? GetService<TService>() => GetServiceProvider().GetService<TService>();

    public TService GetRequiredService<TService>() where TService : notnull
        => GetServiceProvider().GetRequiredService<TService>();

#pragma warning disable CA2208
    protected virtual IServiceProvider GetServiceProvider()
        => MasaApp.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices ??
            throw new MasaException("Failed to get ServiceProvider of current request");
#pragma warning restore CA2208

    internal void AutoMapRoute(ServiceGlobalRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        var type = GetType();

        var methodInfos = type
            .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            .Where(methodInfo => methodInfo.CustomAttributes.All(attr => attr.AttributeType != typeof(IgnoreRouteAttribute)))
            .Concat(type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(methodInfo => methodInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(RoutePatternAttribute))))
            .Distinct();

        foreach (var method in methodInfos)
        {
            var handler = ServiceBaseHelper.CreateDelegate(method, this);

            string? pattern = null;
            string? httpMethod = null;
            string? methodName = null;
            var attribute = method.GetCustomAttribute<RoutePatternAttribute>();
            if (attribute != null)
            {
                httpMethod = attribute.HttpMethod;
                if (attribute.StartWithBaseUri)
                    methodName = attribute.Pattern;
                else
                    pattern = attribute.Pattern;
            }

            string newMethodName = method.Name;

            if (httpMethod == null || pattern == null)
            {
                var result = ParseMethod(globalOptions, newMethodName);
                httpMethod ??= result.HttpMethod;
                newMethodName = result.MethodName;
            }

            pattern ??= ServiceBaseHelper.CombineUris(GetBaseUri(globalOptions, pluralizationService),
                methodName ?? GetMethodName(method, newMethodName, globalOptions));
            var routeHandlerBuilder = MapMethods(globalOptions, pattern, httpMethod, handler);
            (RouteHandlerBuilder ?? globalOptions.RouteHandlerBuilder)?.Invoke(routeHandlerBuilder);
        }
    }

    protected virtual string GetBaseUri(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        if (!string.IsNullOrWhiteSpace(BaseUri))
            return BaseUri;

        var list = new List<string>()
        {
            RouteOptions.Prefix ?? globalOptions.Prefix ?? string.Empty,
            RouteOptions.Version ?? globalOptions.Version ?? string.Empty,
            ServiceName ?? GetServiceName(RouteOptions.PluralizeServiceName ?? globalOptions.PluralizeServiceName ?? false ?
                pluralizationService :
                null)
        };

        return string.Join('/', list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(u => u.Trim('/')));
    }

    RouteHandlerBuilder MapMethods(ServiceRouteOptions globalOptions, string pattern, string? httpMethod, Delegate handler)
    {
        if (httpMethod != null)
            return App.MapMethods(pattern, new[] { httpMethod }, handler);

        var httpMethods = GetDefaultHttpMethods(globalOptions);
        if (httpMethods.Length > 0)
            return App.MapMethods(pattern, httpMethods, handler);

        return App.Map(pattern, handler);
    }

    protected virtual string[] GetDefaultHttpMethods(ServiceRouteOptions globalOptions)
    {
        if (RouteOptions.DefaultHttpMethods.Length > 0)
            return RouteOptions.DefaultHttpMethods;

        if (globalOptions.DefaultHttpMethods.Length > 0)
            return globalOptions.DefaultHttpMethods;

        return Array.Empty<string>();
    }

    protected virtual string GetServiceName(PluralizationService? pluralizationService)
    {
        var serviceName = GetType().Name.TrimEnd("Service", StringComparison.OrdinalIgnoreCase);
        if (pluralizationService == null)
            return serviceName;

        return pluralizationService.Pluralize(serviceName);
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected virtual string GetMethodName(MethodInfo methodInfo, string methodName, ServiceRouteOptions globalOptions)
    {
        if (!(RouteOptions.AutoAppendId ?? globalOptions.AutoAppendId ?? false))
            return ServiceBaseHelper.TrimEndMethodName(methodName);

        var idParameter = methodInfo.GetParameters().FirstOrDefault(p => p.Name!.Equals("id", StringComparison.OrdinalIgnoreCase) &&
            p.GetCustomAttribute<FromBodyAttribute>() == null &&
            p.GetCustomAttribute<FromFormAttribute>() == null &&
            p.GetCustomAttribute<FromHeaderAttribute>() == null &&
            p.GetCustomAttribute<FromQueryAttribute>() == null &&
            p.GetCustomAttribute<FromServicesAttribute>() == null);
        if (idParameter != null)
        {
            var id = idParameter.ParameterType.IsNullableType() || idParameter.HasDefaultValue ? "{id?}" : "{id}";
            return $"{ServiceBaseHelper.TrimEndMethodName(methodName)}/{id}";
        }

        return ServiceBaseHelper.TrimEndMethodName(methodName);
    }

    protected virtual (string? HttpMethod, string MethodName) ParseMethod(ServiceRouteOptions globalOptions, string methodName)
    {
        var prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.GetPrefixes ?? globalOptions.GetPrefixes!, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("GET", ParseMethodName());

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.PostPrefixes ?? globalOptions.PostPrefixes!, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("POST", ParseMethodName());

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.PutPrefixes ?? globalOptions.PutPrefixes!, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("PUT", ParseMethodName());

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.DeletePrefixes ?? globalOptions.DeletePrefixes!, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("DELETE", ParseMethodName());

        return (null, methodName);

        string ParseMethodName()
        {
            if (RouteOptions.DisableTrimStartMethodPrefix ?? globalOptions.DisableTrimStartMethodPrefix ?? false)
                return methodName;

            return methodName.Substring(prefix.Length);
        }
    }

    #region Obsolete

#pragma warning disable S4136
    [Obsolete("service can be ignored")]
    protected ServiceBase(IServiceCollection services)
    {
    }

    [Obsolete("service can be ignored")]
    protected ServiceBase(IServiceCollection services, string baseUri) : this(baseUri)
    {
    }
#pragma warning restore S4136

    #region [Obsolete] Map GET,POST,PUT,DELETE

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="Builder.RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapGet(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= ServiceBaseHelper.TrimEndMethodName(handler.Method.Name);

        var pattern = ServiceBaseHelper.CombineUris(BaseUri, customUri);

        return App.MapGet(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP POST requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="Builder.RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapPost(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= ServiceBaseHelper.TrimEndMethodName(handler.Method.Name);

        var pattern = ServiceBaseHelper.CombineUris(BaseUri, customUri);

        return App.MapPost(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PUT requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="Builder.RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapPut(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= ServiceBaseHelper.TrimEndMethodName(handler.Method.Name);

        var pattern = ServiceBaseHelper.CombineUris(BaseUri, customUri);

        return App.MapPut(pattern, handler);
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP DELETE requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="Builder.RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapDelete(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= ServiceBaseHelper.TrimEndMethodName(handler.Method.Name);

        var pattern = ServiceBaseHelper.CombineUris(BaseUri, customUri);

        return App.MapDelete(pattern, handler);
    }

    #endregion

    #endregion

}
