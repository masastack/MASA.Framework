// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public class AddCallerExtensions
{
    public static void AddCaller(CallerOptions callerOptions, string name, bool isDefault, Func<IServiceProvider, ICallerProvider> func)
    {
        if (callerOptions.Callers.Any(c => c.Name == name))
            throw new ArgumentException($"The caller name already exists, please change the name, the repeat name is {name}");

        callerOptions.Callers.Add(new CallerRelations(name, isDefault, func));
    }
}
