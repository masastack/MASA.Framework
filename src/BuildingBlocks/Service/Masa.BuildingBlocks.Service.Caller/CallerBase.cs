// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class CallerBase
{
    public virtual string? Name { get; private set; } = null;

    protected CallerOptionsBuilder CallerOptions { get; private set; } = default!;

    private ICaller? _caller;

    protected ICaller Caller => GetCaller();

    [Obsolete("CallerProvider has expired, please use Caller")]
    protected ICaller CallerProvider => Caller;

    public IServiceProvider? ServiceProvider { get; private set; }

    protected CallerBase() => ServiceProvider = null;

    protected CallerBase(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    protected virtual ICaller GetCaller()
        => _caller ??= ServiceProvider!.GetRequiredService<ICallerFactory>().Create(Name!);

    public abstract void UseCallerExtension();

    public void SetCallerOptions(CallerOptionsBuilder callerOptionsBuilder, string name)
    {
        CallerOptions = callerOptionsBuilder;
        Name = name;
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected virtual Task ConfigHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        var authenticationService = ServiceProvider!.GetService<IAuthenticationService>();
        authenticationService?.ExecuteAsync(requestMessage);
        return Task.CompletedTask;
    }
}
