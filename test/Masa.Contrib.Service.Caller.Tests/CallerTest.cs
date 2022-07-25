// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

[TestClass]
public class CallerTest
{
    private WebApplicationBuilder _builder = default!;

    [TestInitialize]
    public void EdgeDriverInitialize()
    {
        _builder = WebApplication.CreateBuilder();
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        _builder.Services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(httpClientBuilder =>
            {
                httpClientBuilder.Configure = builder => builder.Timeout = TimeSpan.FromSeconds(3);
                httpClientBuilder.BaseAddress = "https://github.com/masastack";
            });
        });
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var githubCaller = serviceProvider.GetRequiredService<ICallerProvider>();
        Assert.IsTrue(await GetAsync(githubCaller));
    }

    private async Task<bool> GetAsync(ICallerProvider callerProvider)
    {
        var res = await callerProvider.GetAsync("");
        return res.IsSuccessStatusCode && res.StatusCode == HttpStatusCode.OK;
    }

    [TestMethod]
    public void TestConvertToDictionaryByDynamic()
    {
        var provider = new DefaultTypeConvertProvider();
        var dictionary = new Dictionary<string, string>
        {
            { "account", "jim" },
            { "age", "18" }
        };
        var request = new
        {
            account = "jim",
            age = 18
        };
        var result = provider.ConvertToDictionary(request);
        Assert.IsTrue(System.Text.Json.JsonSerializer.Serialize(result) == System.Text.Json.JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserListQuery("Jim");
        var dictionary = new Dictionary<string, string>
        {
            { "name", query.Name }
        };
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(System.Text.Json.JsonSerializer.Serialize(result) == System.Text.Json.JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject2()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserDetailQuery("Jim", "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 2);
        Assert.IsTrue(result["name"] == query.Name);
        Assert.IsTrue(result["Tags"] == System.Text.Json.JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject3()
    {
        var provider = new DefaultTypeConvertProvider();

        List<string> tags = null!;
        var query = new UserDetailQuery("Jim", tags);
        var result = provider.ConvertToDictionary(query);

        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["name"] == query.Name);
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject4()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserDetailQuery(null!, "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Tags"] == System.Text.Json.JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject5()
    {
        var provider = new DefaultTypeConvertProvider();
        var dic = new Dictionary<string, string>()
        {
            { "Account", "Jim" }
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject6()
    {
        var provider = new DefaultTypeConvertProvider();
        var dic = new List<KeyValuePair<string, string>>()
        {
            new("Account", "Jim")
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }
}
