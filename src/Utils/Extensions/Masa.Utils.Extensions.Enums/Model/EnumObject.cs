// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class EnumObject<TValue>
{
    public string Name { get; set; } = default!;

    public TValue Value { get; set; } = default!;
}
