// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Contracts;

public abstract class BaseResponse : BaseMessage
{
    protected BaseResponse()
    {
    }

    protected BaseResponse(Guid correlationId) : base()
    {
        base._correlationId = correlationId;
    }
}

public abstract class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    protected BaseResponse()
    {
    }

    protected BaseResponse(Guid correlationId)
        : base(correlationId)
    {
    }
}
