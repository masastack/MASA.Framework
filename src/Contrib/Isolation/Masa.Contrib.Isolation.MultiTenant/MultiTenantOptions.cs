// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant;

public class MultiTenantOptions
{
    public string TenantKey { get; set; }

    public List<IParserProvider> ParserProviders { get; set; }
}
