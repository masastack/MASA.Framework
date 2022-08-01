// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Response;

[Serializable]
[XmlRoot]
public class BaseResponse
{
    [XmlElement]
    public string Code { get; set; } = default!;

    public BaseResponse() { }

    public BaseResponse(string code) { Code = code; }
}
