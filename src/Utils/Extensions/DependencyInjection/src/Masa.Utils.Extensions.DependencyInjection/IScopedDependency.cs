// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public interface IScopedDependency
{
}


public interface ITestService<T> : IScopedDependency
    where T : class
{

}

public class TestSerice<T> : ITestService<T>
    where T : class
{

}
