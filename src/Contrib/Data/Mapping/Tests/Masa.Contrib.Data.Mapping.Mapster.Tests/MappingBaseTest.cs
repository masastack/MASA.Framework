// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests;

public class MappingBaseTest
{
    protected IServiceCollection _services;
    protected IMapper _mapper = default!;

    protected MappingBaseTest()
    {
        _services = new ServiceCollection();
        _services.AddMapster();
        var serviceProvider = _services.BuildServiceProvider();
        _mapper = serviceProvider.GetRequiredService<IMapper>();
    }
}
