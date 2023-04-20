// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Entities;

public class OperationLog : FullEntity<Guid, int>
{
    public string Name { get; set; }

    public Guid GoodsId { get; private set; } = default!;

    public Goods Goods { get; private set; } = default!;

    public OperationLog()
    {
    }

    public OperationLog(string name) : this()
    {
        Name = name;
    }

    public void SetDeleted(bool isDeleted, int modifier, DateTime modificationTime)
    {
        IsDeleted = isDeleted;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}
