// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class CatalogService : CustomServiceBase
{
    public CatalogService()
    {
        ServiceName = "catalog";
    }

    public CatalogService(string baseUri)
    {
        BaseUri = baseUri;
    }
}
