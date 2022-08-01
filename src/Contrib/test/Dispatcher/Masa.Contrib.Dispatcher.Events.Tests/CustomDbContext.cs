// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

public class CustomDbContext : MasaDbContext
{
    public CustomDbContext(MasaDbContextOptions options) : base(options) { }
}
