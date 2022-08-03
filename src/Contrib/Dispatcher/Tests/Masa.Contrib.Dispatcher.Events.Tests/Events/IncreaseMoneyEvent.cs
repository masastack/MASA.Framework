// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record IncreaseMoneyEvent : Event, ITransaction
{
    public IUnitOfWork? UnitOfWork { get; set; }

    public string Account { get; set; }

    public string TransferAccount { get; set; }

    public decimal Money { get; set; }
}
