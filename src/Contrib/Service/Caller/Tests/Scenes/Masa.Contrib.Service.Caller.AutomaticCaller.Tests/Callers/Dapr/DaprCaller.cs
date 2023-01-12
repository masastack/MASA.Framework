// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public class DaprCaller : UserDaprCallerBase
{
    public DaprCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public bool CallerProviderIsNotNull() => Caller != null!;

    public ICaller GetCaller() => Caller;
}
