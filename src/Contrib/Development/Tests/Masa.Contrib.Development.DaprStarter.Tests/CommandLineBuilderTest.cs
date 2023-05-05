// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.Tests;

[TestClass]
public class CommandLineBuilderTest
{
    [DataTestMethod]
    [DataRow(null, null, "--app-id test")]
    [DataRow(0, 0, "--app-id test")]
    [DataRow(5001, 0, "--app-id test --dapr-grpc-port 5001")]
    [DataRow(0, 5000, "--app-id test --dapr-http-port 5000")]
    [DataRow(5001, 5000, "--app-id test --dapr-grpc-port 5001 --dapr-http-port 5000")]
    public void TestToString(int? grpcPort, int? httpPort, string expectedResult)
    {
        var builder = new CommandLineBuilder(DaprStarterConstant.DEFAULT_ARGUMENT_PREFIX);
        builder
            .Add("app-id", () => "test", false)
            .Add("dapr-grpc-port", () => grpcPort?.ToString() ?? string.Empty, !(grpcPort > 0))
            .Add("dapr-http-port", () => httpPort?.ToString() ?? string.Empty, !(httpPort > 0));
        var result = builder.ToString();
        Assert.AreEqual(expectedResult, result);
    }
}
