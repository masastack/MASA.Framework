// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Tests.Commands;

internal record OrderCommand : Command
{
    public int Count { get; set; }
}
