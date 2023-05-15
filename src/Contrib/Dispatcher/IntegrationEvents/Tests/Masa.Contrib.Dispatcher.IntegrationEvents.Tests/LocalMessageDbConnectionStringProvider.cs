﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

public class LocalMessageDbConnectionStringProvider : ILocalMessageDbConnectionStringProvider
{
    public List<string> ConnectionStrings => new()
    {
        ""
    };
}
