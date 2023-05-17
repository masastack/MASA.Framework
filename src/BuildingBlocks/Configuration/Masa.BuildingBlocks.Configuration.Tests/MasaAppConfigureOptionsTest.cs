// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Tests;

[TestClass]
public class MasaAppConfigureOptionsTest
{
    // private static readonly string DefaultAppId = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-");
    //
    // [DataRow("", "", "cluster", "", "Production", "cluster")]
    // [DataRow("", "environment", "", "", "environment", "Default")]
    // [DataRow("", "", "", "", "Production", "Default")]
    // [DataRow("appid", "", "", "appid", "Production", "Default")]
    // [DataRow("appid", "environment", "cluster", "appid", "environment", "cluster")]
    // [DataTestMethod]
    // public void Test(
    //     string inputAppId, string inputEnvironment, string inputCluster,
    //     string expectedAppId, string expectedEnvironment, string expectedCluster)
    // {
    //     var masaAppConfigureOptions = new MasaAppConfigureOptions
    //     {
    //         AppId = inputAppId,
    //         Environment = inputEnvironment,
    //         Cluster = inputCluster
    //     };
    //     Assert.AreEqual(!string.IsNullOrWhiteSpace(inputAppId) ? expectedAppId : DefaultAppId, masaAppConfigureOptions.AppId);
    //     Assert.AreEqual(expectedEnvironment, masaAppConfigureOptions.Environment);
    //     Assert.AreEqual(expectedCluster, masaAppConfigureOptions.Cluster);
    // }
}
