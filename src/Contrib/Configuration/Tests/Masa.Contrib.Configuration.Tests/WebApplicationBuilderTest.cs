// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class WebApplicationBuilderTest
{
// #pragma warning disable CS0618
//     [TestMethod]
//     public void TestInitializeAppConfiguration()
//     {
//         var builder = WebApplication.CreateBuilder();
//         string env = "Development";
//         builder.Services.Configure<MasaAppConfigureOptions>(options =>
//         {
//             options.Environment = env;
//         });
//         builder.InitializeAppConfiguration();
//         var serviceProvider = builder.Services.BuildServiceProvider();
//         var masaAppConfigureOptions = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()!;
//
//         Assert.IsTrue(masaAppConfigureOptions.Value.Length == 3);
//         Assert.IsTrue(masaAppConfigureOptions.Value.Environment == env);
//         Assert.IsTrue(masaAppConfigureOptions.Value.GetValue(nameof(MasaAppConfigureOptions.Environment)) == env);
//     }
// #pragma warning restore CS0618
//
//     [TestMethod]
//     public void TestInitializeAppConfiguration2()
//     {
//         var builder = WebApplication.CreateBuilder();
//         string env = "Development";
//         builder.Services.Configure<MasaAppConfigureOptions>(options =>
//         {
//             options.Environment = env;
//         });
//         builder.Services.InitializeAppConfiguration();
//         var serviceProvider = builder.Services.BuildServiceProvider();
//         var masaAppConfigureOptions = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()!;
//
//         Assert.IsTrue(masaAppConfigureOptions.Value.Length == 3);
//         Assert.IsTrue(masaAppConfigureOptions.Value.Environment == env);
//         Assert.IsTrue(masaAppConfigureOptions.Value.GetValue(nameof(MasaAppConfigureOptions.Environment)) == env);
//     }
//
//     [TestMethod]
//     public void TestMigrateRelations()
//     {
//         var builder = WebApplication.CreateBuilder();
//         builder.Services.AddMasaConfiguration(configurationBuilder =>
//         {
//             configurationBuilder.AddJsonFile("rabbitMq.json");
//             configurationBuilder.AddJsonFile("logging.json");
//         }, options =>
//         {
//             options.Assemblies = new[]
//             {
//                 typeof(ConfigurationTest).Assembly
//             };
//             options.MigrateRelations.Add(new MigrateConfigurationRelationsInfo(
//                 "DefaultLogging",
//                 $"{nameof(SectionTypes.Local)}{ConfigurationPath.KeyDelimiter}DefaultLogging"));
//         });
//         Assert.AreEqual("Information", builder.Configuration["Logging:LogLevel:Default"]);
//         Assert.AreEqual("Warning", builder.Configuration["Logging:LogLevel:Microsoft"]);
//         Assert.AreEqual("Information", builder.Configuration["Logging:LogLevel:Microsoft.Hosting.Lifetime"]);
//         Assert.AreEqual("Test", builder.Configuration["DefaultLogging"]);
//         Assert.AreEqual("Test", builder.Services.GetMasaConfiguration().Local["DefaultLogging"]);
//     }
}
