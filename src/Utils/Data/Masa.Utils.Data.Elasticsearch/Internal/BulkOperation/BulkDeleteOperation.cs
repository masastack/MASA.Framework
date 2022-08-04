// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Internal.BulkOperation;

public class BulkDeleteOperation : BulkOperationBase
{
    public BulkDeleteOperation(Id id) => Id = id;

    protected override object GetBody() => null!;

    protected override Type ClrType { get; } = default!;

    protected override string Operation => "delete";
}
