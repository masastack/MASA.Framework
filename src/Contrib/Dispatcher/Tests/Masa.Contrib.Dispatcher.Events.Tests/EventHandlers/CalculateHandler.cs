// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests;

#pragma warning disable CA1822
public class CalculateHandler
{
    [EventHandler(Order = 1)]
    public void First(CalculateEvent @event)
    {
        @event.Result = @event.ParameterA + @event.ParameterB;
        if (@event.Result % 2 == 0)
            throw new Exception("result is even");
    }

    [EventHandler(Order = 1, IsCancel = true)]
    public void FirstCancel(CalculateEvent @event)
    {
        @event.Result -= 3;
    }

    [EventHandler(Order = 2, FailureLevels = FailureLevels.Throw)]
    public void Second(CalculateEvent @event)
    {
        @event.Result *= 3;

        if (@event.Result / 12 > 0)
            throw new Exception("result must not be greater than 11");
    }

    [EventHandler(Order = 2, IsCancel = true)]
    public void SecondCancel(CalculateEvent @event)
    {
        @event.Result = @event.Result - 2;
    }

    [EventHandler(Order = 3, FailureLevels = FailureLevels.ThrowAndCancel)]
    public void Third(CalculateEvent @event)
    {
        if (@event.Result == 9)
            throw new Exception("result is not equal to 9");
    }

    [EventHandler(Order = 3, IsCancel = true)]
    public void ThirdCancel(CalculateEvent @event)
    {
        @event.Result -= 5;
    }
}
#pragma warning restore CA1822
