﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class OrderService: CustomServiceBase
{
    public OrderService(IServiceCollection services) : base(services, "/api/order")
    {
        DisableRestful = true;
    }
}