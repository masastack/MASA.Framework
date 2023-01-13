// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class CallerBase
{
    public virtual string? Name { get; set; }

    protected CallerOptions CallerOptions { get; private set; } = default!;

    private ICaller? _caller;

    protected ICaller Caller => _caller ??= ServiceProvider!.GetRequiredService<ICallerFactory>().Create(Name!);

    [Obsolete("CallerProvider has expired, please use Caller")]
    protected ICaller CallerProvider => Caller;

    public IServiceProvider? ServiceProvider { get; private set; }

    protected CallerBase() => ServiceProvider = null;

    protected CallerBase(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public abstract void UseCallerExtension();

    public void SetCallerOptions(CallerOptions options, string name)
    {
        CallerOptions = options;
        Name ??= name;
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected virtual Task ConfigHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        return Task.CompletedTask;
    }
}
