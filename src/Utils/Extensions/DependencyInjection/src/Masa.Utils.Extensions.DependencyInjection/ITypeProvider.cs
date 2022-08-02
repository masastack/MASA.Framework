// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public interface ITypeProvider
{
    bool IsAssignableFrom(Type type, Type targetType);

    bool IsAssignableTo(Type type, Type targetType);

    List<Type> GetAllTypes();

    List<Type> GetAllTypes(IEnumerable<Assembly> assemblies);

    List<ServiceDescriptorOptions> GetServiceDescriptors(List<Type> types);
}
