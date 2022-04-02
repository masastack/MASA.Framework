using Masa.BuildingBlocks.BasicAbility.Pm;
using Masa.BuildingBlocks.BasicAbility.Pm.Model;
using Masa.Utils.Caching.Redis.Models;
using Masa.Utils.Caller.Core;
using Masa.Utils.Caller.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Masa.Contrib.BasicAbility.Pm.Tests
{
    [TestClass]
    public class PmCachingTest
    {
        private Mock<ICallerProvider> _callerProvider;
        private IPmCaching _pmCaching;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();

            //Mock<IHttpClientFactory> httpClientFactory = new();
            //services.AddSingleton(httpClientFactory.Object);
            //services.AddCaller(options => options.UseHttpClient());

            //services.AddPmCaching(callerOptions =>
            //{
            //    callerOptions.UseHttpClient(builder =>
            //    {
            //        builder.Name = "CustomHttpClient";
            //        builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            //    });
            //});

            //var serviceProvider = services.BuildServiceProvider();
            //_pmCaching = serviceProvider.GetRequiredService<IPmCaching>();

            _callerProvider = new Mock<ICallerProvider>();

        }

        [TestMethod]
        public async Task GetProjectAppsListAsync()
        {
            List<ProjectModel> list = new List<ProjectModel>()
            {
                new ProjectModel(1,"","",1,Guid.NewGuid())
            };
            string env = "development";
            var requestUri = $"third-party/api/v1/env/{env}";
            _callerProvider.Setup(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default).Result).Returns(list).Verifiable();
            var pmCaching = new PmCaching(_callerProvider.Object);

            var data = await pmCaching.GetProjectListAsync(env);
            _callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);
            Assert.IsTrue(data.Count == 1);
        }

        [TestMethod]
        public async Task GetProjectAppsList2Async()
        {
            List<ProjectModel> list = null!;
            string env = "development";
            var requestUri = $"third-party/api/v1/env/{env}";
            _callerProvider.Setup(provider => provider.GetAsync<List<ProjectModel>>(It.IsAny<string>(), default).Result).Returns(list).Verifiable();
            var pmCaching = new PmCaching(_callerProvider.Object);

            var data = await pmCaching.GetProjectListAsync(env);
            _callerProvider.Verify(provider => provider.GetAsync<List<ProjectModel>>(requestUri, default), Times.Once);
            Assert.IsTrue(data.Count == 0);
        }
    }
}
