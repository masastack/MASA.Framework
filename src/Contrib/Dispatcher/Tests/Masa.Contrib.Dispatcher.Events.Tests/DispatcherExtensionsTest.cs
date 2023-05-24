// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class DispatcherExtensionsTest
{
    [TestMethod]
    public void TestIsGeneric()
    {
        Type listGenericType = typeof(List<>);
        bool result = GetResult(listGenericType);
    }

    public static bool GetResult( Type type) => type.GetTypeInfo().IsGenericTypeDefinition != type.GetTypeInfo().ContainsGenericParameters;
}
