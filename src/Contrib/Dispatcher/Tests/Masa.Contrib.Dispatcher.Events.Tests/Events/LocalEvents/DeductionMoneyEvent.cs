// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record DeductionMoneyEvent : Event, ITransaction
{
    public IUnitOfWork? UnitOfWork { get; set; }

    public string Account { get; set; }

    public string PayeeAccount { get; set; }

    public decimal Money { get; set; }
}
