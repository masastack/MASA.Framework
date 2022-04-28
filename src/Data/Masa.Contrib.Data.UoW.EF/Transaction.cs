// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EF;

public class Transaction : ITransaction
{
    public Transaction(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }
}
