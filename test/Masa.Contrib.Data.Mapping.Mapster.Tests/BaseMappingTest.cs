// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests;

[TestClass]
public class BaseMappingTest
{
    protected IServiceCollection _services;
    protected IMapper _mapper = default!;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddMapping();
        var serviceProvider = _services.BuildServiceProvider();
        _mapper = serviceProvider.GetRequiredService<IMapper>();
    }
}
