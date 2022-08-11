// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Model;

internal abstract class Property
{
    public string Key { get; set; } = default!;

    public string Value { get; set; } = default!;
}
