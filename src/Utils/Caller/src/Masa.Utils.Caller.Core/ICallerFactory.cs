// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public interface ICallerFactory
{
    ICallerProvider CreateClient();

    ICallerProvider CreateClient(string name);
}
