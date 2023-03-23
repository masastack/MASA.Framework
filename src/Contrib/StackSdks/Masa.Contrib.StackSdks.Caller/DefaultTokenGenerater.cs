// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

internal class DefaultTokenGenerater : ITokenGenerater
{
    public TokenProvider Generater()
    {
        return new TokenProvider();
    }
}
