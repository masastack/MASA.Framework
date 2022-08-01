// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public abstract class CallerBase
{
    public virtual string Name { get; set; } = string.Empty;

    protected CallerOptions CallerOptions { get; private set; } = default!;

    private ICallerProvider? _callerProvider;

    protected ICallerProvider CallerProvider => _callerProvider ??= ServiceProvider.GetRequiredService<ICallerFactory>().CreateClient(Name);

    private IServiceProvider ServiceProvider { get; }

    protected CallerBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public abstract void UseCallerExtension();

    public void SetCallerOptions(CallerOptions options, string name)
    {
        CallerOptions = options;
        if (string.IsNullOrEmpty(Name))
            Name = name;
    }
}
