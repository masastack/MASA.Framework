// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EFCore.Web.Tests.Events;

public record RegisterUserEvent(string Account,string Password) : Event
{
}
