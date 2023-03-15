// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public interface ITokenProvider
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public string? IdToken { get; set; }
}
