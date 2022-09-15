// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Builder;

public abstract class ServiceBase : ServiceRouteOptions, IService
{
    public WebApplication App => MasaApp.GetRequiredService<WebApplication>();

    public string BaseUri { get; init; }

    public string? ServiceName { get; init; }

    public bool DisableRestful { get; init; } = MasaService.DisableRestful;

    public IServiceCollection Services => MasaApp.Services;

    protected ServiceBase() { }

    protected ServiceBase(string baseUri)
    {
        BaseUri = baseUri;
    }

    public TService? GetService<TService>() => GetServiceProvider().GetService<TService>();

    public TService GetRequiredService<TService>() where TService : notnull
        => GetServiceProvider().GetRequiredService<TService>();

    protected virtual IServiceProvider GetServiceProvider()
        => MasaApp.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices ?? Services.BuildServiceProvider();

    internal void AutoMapRoute(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        var type = GetType();

        var methodInfos = type
            .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            .Where(methodInfo => methodInfo.CustomAttributes.All(attr => attr.AttributeType != typeof(IgnoreRouteAttribute)));

        foreach (var method in methodInfos)
        {
            var @delegate = CreateDelegate(method, this);

            //todo RoutePatternAttribute

            var pattern = CombineUris(GetBaseUri(globalOptions, pluralizationService), GetMethodName(method, globalOptions));
            var httpMethod = GetHttpMethod(globalOptions, method.Name);

            if (httpMethod != null)
                App.MapMethods(pattern, new[] { httpMethod }, @delegate);
            else
                App.Map(pattern, @delegate);
        }
    }

    private static Delegate CreateDelegate(MethodInfo methodInfo, object targetInstance)
    {
        var type = Expression.GetDelegateType(methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType)
            .Concat(new List<Type>
                { methodInfo.ReturnType }).ToArray());
        return Delegate.CreateDelegate(type, targetInstance, methodInfo);
    }

    public static string CombineUris(params string[] uris)
        => string.Join("/", uris.Select(u => u.Trim('/')));

    protected virtual string GetBaseUri(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        if (DisableRestful) return string.Empty;

        return string.IsNullOrWhiteSpace(BaseUri) ? GetUrl(globalOptions, pluralizationService) : BaseUri;
    }

    private string GetUrl(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        var list = new List<string>()
        {
            Prefix ?? globalOptions.Prefix ?? string.Empty,
            Version ?? globalOptions.Version ?? string.Empty,
            ServiceName ??
            GetServiceName((PluralizeServiceName ?? globalOptions.PluralizeServiceName) is true ? pluralizationService : null)
        };

        return string.Join('/', list.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private string GetServiceName(PluralizationService? pluralizationService)
    {
        var typeName = GetType().Name;
        var index = typeName.LastIndexOf("Service", StringComparison.OrdinalIgnoreCase);
        var serviceName = typeName.Remove(index);
        if (pluralizationService == null)
            return serviceName;

        return pluralizationService.Pluralize(serviceName);
    }

    protected virtual string? GetHttpMethod(ServiceRouteOptions globalOptions, string methodName)
    {
        if (ExistPrefix(GetPrefixs ?? globalOptions.GetPrefixs!, methodName))
            return "GET";

        if (ExistPrefix(PostPrefixs ?? globalOptions.PostPrefixs!, methodName))
            return "POST";

        if (ExistPrefix(PutPrefixs ?? globalOptions.PutPrefixs!, methodName))
            return "PUT";

        if (ExistPrefix(DeletePrefixs ?? globalOptions.DeletePrefixs!, methodName))
            return "DELETE";

        return null;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected virtual string GetMethodName(MethodInfo methodInfo, ServiceRouteOptions globalOptions)
    {
        if (!(AutoAppendId ?? globalOptions.AutoAppendId ?? false))
        {
            return FormatMethodName(globalOptions, methodInfo.Name);
        }

        var parameters = methodInfo.GetParameters();
        var idParameter = parameters.FirstOrDefault(p => p.Name!.Equals("id", StringComparison.OrdinalIgnoreCase));
        if (idParameter != null)
        {
            var id = idParameter.ParameterType.IsNullableType() ? "{id?}" : "{id}";
            return $"{FormatMethodName(globalOptions, methodInfo.Name)}/{id}";
        }

        return FormatMethodName(globalOptions, methodInfo.Name);
    }

    public string TryParseHttpMethod(ServiceRouteOptions globalOptions, string methodName)
    {
        var httpMethod = TryParseHttpMethod(globalOptions, methodName, out var newMethodName);

        if (newMethodName.EndsWith("Async"))
        {
            var i = newMethodName.LastIndexOf("Async", StringComparison.Ordinal);
            newMethodName = newMethodName[..i];
        }

        return newMethodName;
    }

    protected virtual (string? HttpMethod, string MethodName) TryParseHttpMethod(ServiceRouteOptions globalOptions, string methodName)
    {
        if (TrimPrefix(GetPrefixs ?? globalOptions.GetPrefixs!, methodName, out newMethodName))
            return "GET";

        if (TrimPrefix(PostPrefixs ?? globalOptions.PostPrefixs!, methodName, out newMethodName))
            return "POST";

        if (TrimPrefix(PutPrefixs ?? globalOptions.PutPrefixs!, methodName, out newMethodName))
            return "PUT";

        if (TrimPrefix(DeletePrefixs ?? globalOptions.DeletePrefixs!, methodName, out newMethodName))
            return "DELETE";

        newMethodName = methodName;
        return null;
    }

    public bool TrimPrefix(string[] prefixs, string methodName, out string newMethodName)
    {
        var prefix = prefixs.FirstOrDefault(prefix => methodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        if (prefix is not null)
        {
            newMethodName = methodName.Substring(prefix.Length - 1);
            return true;
        }
        else
        {
            newMethodName = methodName;
            return false;
        }
    }

    #region Obsolete

    [Obsolete("service can be ignored")]
    protected ServiceBase(IServiceCollection services)
    {
    }

    [Obsolete("service can be ignored")]
    protected ServiceBase(IServiceCollection services, string baseUri) : this(services)
    {

    }

    #region [Obsolete] Map GET,POST,PUT,DELETE

    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
    /// for the specified pattern, a combination of <see cref="BaseUri"/> and <see cref="customUri"/> or <see cref="handler"/> name.
    /// </summary>
    /// <param name="handler">The delegate executed when the endpoint is matched. It's name is a part of pattern if <see cref="customUri"/> is null.</param>
    /// <param name="customUri">The custom uri. It is a part of pattern if it is not null.</param>
    /// <param name="trimEndAsync">Determines whether to remove the string 'Async' at the end.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapGet(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatMethodName(handler.Method.Name);

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
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapPost(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatMethodName(handler.Method.Name);

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
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapPut(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatMethodName(handler.Method.Name);

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
    [Obsolete("It is recommended to map according to the automatic mapping rules")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected RouteHandlerBuilder MapDelete(Delegate handler, string? customUri = null, bool trimEndAsync = true)
    {
        customUri ??= FormatMethodName(handler.Method.Name);

        var pattern = CombineUris(BaseUri, customUri);

        return App.MapDelete(pattern, handler);
    }

    #endregion

    #endregion

}
