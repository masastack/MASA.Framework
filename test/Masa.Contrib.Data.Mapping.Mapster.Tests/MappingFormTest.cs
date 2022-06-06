// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests;

[TestClass]
public class MappingFormTest : BaseMappingTest
{
    [TestMethod]
    public void TestUseShareModeReturnMapRuleIsExist()
    {
        var request = new CreateUserRequest()
        {
            Name = "Jim",
        };
        _mapper.Map<CreateUserRequest, User>(request);
        Assert.IsTrue(TypeAdapterConfig.GlobalSettings.RuleMap.Any(r
            => r.Key.Source == typeof(CreateUserRequest) && r.Key.Destination == typeof(User)));
    }

    [TestMethod]
    public void TestAddMultiMapping()
    {
        _services.AddMapping();
        var mappers = _services.BuildServiceProvider().GetServices<IMapper>();
        Assert.IsTrue(mappers.Count() == 1);
    }
}
