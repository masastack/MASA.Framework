// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Entities;

public class Goods : FullAggregateRoot<Guid, int>
{
    public string Name { get; set; }

    public List<OperationLog> Logs { get; set; }

    public Goods()
    {
    }

    public Goods(string name, int creator, DateTime creationTime, int modifier, DateTime modificationTime) : this()
    {
        Name = name;
        Creator = creator;
        CreationTime = creationTime;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }

    public void UpdateName(string name, int creator, DateTime creationTime, int modifier, DateTime modificationTime)
    {
        Name = name;
        Creator = creator;
        CreationTime = creationTime;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }

    public void UpdateModifier(int modifier, DateTime modificationTime)
    {
        Modifier = modifier;
        ModificationTime = modificationTime;
    }

    public void SetDeleted(bool isDeleted, int modifier, DateTime modificationTime)
    {
        IsDeleted = isDeleted;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}

public class Goods2 : FullAggregateRoot<Guid, int?>
{
    public string Name { get; set; }

    public Goods2(string name)
    {
        Name = name;
        CreationTime = default(DateTime);
        ModificationTime = default(DateTime);
    }
}
