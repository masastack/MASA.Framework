// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public class DefaultRequestIdGenerator : IRequestIdGenerator
{
    public string NewId() => Guid.NewGuid().ToString();
}
