// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Contracts;

[Obsolete("BaseResponse has expired, please use ResponseBase")]
public abstract class BaseResponse : ResponseBase
{
}

public abstract class ResponseBase : MessageBase
{
    protected ResponseBase()
    {
    }

    protected ResponseBase(Guid correlationId) : base()
    {
        base._correlationId = correlationId;
    }
}

[Obsolete("BaseResponse has expired, please use ResponseBase")]
public abstract class BaseResponse<T> : ResponseBase<T>
{
}

public abstract class ResponseBase<T> : ResponseBase
{
    public T? Data { get; set; }

    protected ResponseBase()
    {
    }

    protected ResponseBase(Guid correlationId)
        : base(correlationId)
    {
    }
}
