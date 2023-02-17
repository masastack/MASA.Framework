// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class Transaction : ITransaction
{
    [NotMapped]
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public Transaction(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;
}
