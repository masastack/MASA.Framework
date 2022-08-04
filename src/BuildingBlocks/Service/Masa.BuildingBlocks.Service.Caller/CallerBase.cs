// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class CallerBase
{
    public virtual string Name { get; set; } = string.Empty;

    protected CallerOptions CallerOptions { get; private set; } = default!;

    private ICaller? _caller;

    protected ICaller Caller => _caller ??= ServiceProvider.GetRequiredService<ICallerFactory>().Create(Name);

    [Obsolete("CallerProvider has expired, please use Caller")]
    protected ICaller CallerProvider => Caller;

    private IServiceProvider ServiceProvider { get; }

    protected CallerBase(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public abstract void UseCallerExtension();

    public void SetCallerOptions(CallerOptions options, string name)
    {
        CallerOptions = options;
        if (string.IsNullOrEmpty(Name))
            Name = name;
    }
}
