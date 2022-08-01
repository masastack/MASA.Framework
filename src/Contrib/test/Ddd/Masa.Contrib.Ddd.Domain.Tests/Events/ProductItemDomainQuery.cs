// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record ProductItemDomainQuery : DomainQuery<string>
{
    public string ProductId { get; set; }

    public override string Result { get; set; }
}
