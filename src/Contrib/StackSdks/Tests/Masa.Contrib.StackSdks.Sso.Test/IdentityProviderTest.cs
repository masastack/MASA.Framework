// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Sso.Test
{
    [TestClass]
    public class IdentityProviderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var services = new ServiceCollection();
            services.AddMasaOpenIdConnect(new MasaOpenIdConnectOptions());
            var identityProvider = services.BuildServiceProvider()
                .GetRequiredService<IIdentityProvider>();

            Assert.IsNotNull(identityProvider);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var services = new ServiceCollection();
            services.AddMasaOpenIdConnect(new MasaOpenIdConnectOptions());
            var identityProvider = services.BuildServiceProvider()
                .GetRequiredService<IIdentityProvider>();

            Assert.IsNotNull(identityProvider);
        }
    }
}
