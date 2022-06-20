// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Identity.IdentityModel;

namespace Masa.Contrib.BasicAbility.Auth.Tests
{
    public class BaseAuthTest
    {
        [TestInitialize]
        public void Initialize()
        {
            IServiceCollection service = new ServiceCollection();
            service.AddMasaIdentityModel(IdentityType.MultiEnvironment);
            service.AddAuthClient("https://localhost:18102");
            var authClient = service.BuildServiceProvider().GetRequiredService<IAuthClient>();
        }
    }
}
