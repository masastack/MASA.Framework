// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso.Token;

public class TokenProfile
{
    public string GrantType { get; set; } = default!;

    public List<KeyValuePair<string, string>> Parameters { get; set; } = new();
}
