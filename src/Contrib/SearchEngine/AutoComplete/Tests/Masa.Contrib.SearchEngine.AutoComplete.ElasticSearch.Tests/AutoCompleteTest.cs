// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch.Tests;

[TestClass]
public class AutoCompleteTest
{
    private readonly string _defaultNode = "http://localhost:9200";
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void TestAddAutoComplete()
    {
        string indexName = $"index_{Guid.NewGuid()}";
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting =>
                {
                    setting.EnableApiVersioningHeader(false);
                    setting.DefaultIndex(indexName);
                });
                options.IndexName = indexName;
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var serviceProvider = _services.BuildServiceProvider();

        var autoCompleteFactory = serviceProvider.GetService<IAutoCompleteFactory>();
        Assert.IsNotNull(autoCompleteFactory);

        var autoCompleteClient = serviceProvider.GetService<IAutoCompleteClient>();
        Assert.IsNotNull(autoCompleteClient);
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new("张三", 1),
            new("李四", 2),
            new("张丽", 3)
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("三");
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zs");
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zhang");
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("li");
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("si", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zhang", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(response.IsValid && response.Total == 0);
    }

    [TestMethod]
    public async Task TestMultiConditionsAsyncReturnTotalIs2()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new("张三", 1),
            new("张丽", 2),
            new("李四", 3)
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("张三 ls");
        Assert.IsTrue(response.Total == 2);
        Assert.IsTrue(response.Data[0].Value == 1);
        Assert.IsTrue(response.Data[1].Value == 3);
    }

    [TestMethod]
    public async Task TestMultiConditionsAsyncReturnTotalIs1()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultOperator = Operator.And;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new("张三", 1),
            new("李四", 2),
            new("张丽", 3)
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("张 li");
        Assert.IsTrue(response.Total == 1);
        Assert.IsTrue(response.Data[0].Value == 3);
    }

    [TestMethod]
    public async Task TestDisableMultiConditionsAsyncReturnTotalIs0()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultOperator = Operator.And;
                options.EnableMultipleCondition = false;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();

        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new("张三", 1),
            new("李四", 2),
            new("张丽", 3),
            new("唐伯虎", 4),
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("唐 虎");
        Assert.IsTrue(response.Total == 0);

        var response2 = await autoCompleteClient.GetAsync<long>("唐");
        Assert.IsTrue(response2.Total == 1);

        var response3 = await autoCompleteClient.GetAsync<long>("zs");
        Assert.IsTrue(response3.Total == 1);

        var response4 = await autoCompleteClient.GetAsync<long>("tang");
        Assert.IsTrue(response4.Total == 1);
    }

    [TestMethod]
    public async Task TestCustomModelAsync()
    {
        string analyzer = "ik_max_word_pinyin";

        _services.AddAutoCompleteBySpecifyDocument<Employee>("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultOperator = Operator.And;
                options.DefaultSearchType = SearchType.Precise;
                options.Action = descriptor =>
                {
                    TypeMappingDescriptor<Employee> mappingDescriptor = (TypeMappingDescriptor<Employee>)descriptor;
                    mappingDescriptor.AutoMap<Employee>()
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Text)
                                    .Analyzer(analyzer)
                            )
                        )
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Phone)
                                    .Analyzer(analyzer)
                            )
                        )
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Name)
                                    .Analyzer(analyzer)
                            )
                        );
                };
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var employeeClient = autoCompleteFactory.CreateClient();

        await VerifyByCustomModelAsync(employeeClient);
    }

    [TestMethod]
    public async Task TestCustomModel2Async()
    {
        string analyzer = "ik_max_word_pinyin";
        _services.AddAutoCompleteBySpecifyDocument<Employee>("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultOperator = Operator.And;
                options.DefaultSearchType = SearchType.Precise;
                options.Mapping<Employee>(mappingDescriptor =>
                {
                    mappingDescriptor.AutoMap<Employee>()
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Text)
                                    .Analyzer(analyzer)
                            )
                        )
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Phone)
                                    .Analyzer(analyzer)
                            )
                        )
                        .Properties(ps =>
                            ps.Text(s =>
                                s.Name(n => n.Name)
                                    .Analyzer(analyzer)
                            )
                        );
                });
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var employeeClient = autoCompleteFactory.CreateClient();
        await VerifyByCustomModelAsync(employeeClient);
    }

    private static async Task VerifyByCustomModelAsync(IAutoCompleteClient autoCompleteClient)
    {
        await autoCompleteClient.BuildAsync();

        await autoCompleteClient.SetBySpecifyDocumentAsync(new Employee[]
        {
            new()
            {
                Name = "吉姆",
                Id = 1,
                Phone = "13999999999"
            },
            new()
            {
                Name = "托尼",
                Id = 2,
                Phone = "13888888888"
            }
        });

        await Task.Delay(1000);

        var employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("吉姆");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("139");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 0);

        employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("139*", new AutoCompleteOptions(SearchType.Fuzzy)
        {
            Field = "phone"
        });
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("*", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(employeeResponse.Data.All(employee => employee.Phone == "13999999999" || employee.Phone == "13888888888"));

        await autoCompleteClient.SetBySpecifyDocumentAsync(new Employee
        {
            Name = "吉姆",
            Id = 1,
            Phone = "13777777777"
        });

        await Task.Delay(1000);

        employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("*", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 2);
        Assert.IsTrue(employeeResponse.Data.Any(employee => employee.Phone == "13777777777"));

        await autoCompleteClient.SetBySpecifyDocumentAsync(new Employee
        {
            Name = "吉姆",
            Id = 1,
            Phone = "13999999999"
        }, new SetOptions()
        {
            IsOverride = false
        });

        await Task.Delay(1000);

        employeeResponse = await autoCompleteClient.GetBySpecifyDocumentAsync<Employee>("*", new AutoCompleteOptions(SearchType.Fuzzy));
        Assert.IsTrue(employeeResponse.Data.Any(employee => employee.Phone == "13777777777"));
    }

    [TestMethod]
    public async Task TestPreciseAsync()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var employeeClient = autoCompleteFactory.CreateClient();
        await employeeClient.BuildAsync();

        await employeeClient.SetAsync<Employee, int>(new[]
        {
            new Employee()
            {
                Name = "吉姆",
                Id = 1,
                Phone = "13999999999"
            },
            new Employee()
            {
                Name = "托尼",
                Id = 2,
                Phone = "13888888888"
            }
        });

        await Task.Delay(1000);

        var employeeResponse = await employeeClient.GetAsync<Employee, int>("ji*", new(SearchType.Fuzzy));
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("13999999999", new()
        {
            Field = "phone"
        });
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1 &&
            employeeResponse.Data.Any(employee => employee.Id == 1));
    }

    [TestMethod]
    public async Task TestOperatorAndAsync()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "Edward Adam Davis",
                Value = 1
            },
            new()
            {
                Text = "Edward Jim",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("Edward Adam Davis");
        Assert.IsTrue(response.IsValid && response.Total == 1);
    }

    [TestMethod]
    public async Task TestOperatorOrAsync()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.Or;
            });
        });

        var autoCompleteFactory = _services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "Edward Adam Davis",
                Value = 1
            },
            new()
            {
                Text = "Edward Jim",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.GetAsync<long>("Edward Adam Davis", new(SearchType.Fuzzy));
        Assert.IsTrue(response.IsValid && response.Total == 2);
    }

    [TestMethod]
    public async Task DeleteAsyncReturnDocumentIsNotExist()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));

                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三",
                Value = 1
            },
            new()
            {
                Text = "李四",
                Value = 2
            }
        });
        await Task.Delay(1000);

        var response = await autoCompleteClient.DeleteAsync(10);
        Assert.IsTrue(!response.IsValid);
    }

    [TestMethod]
    public async Task DeleteAsyncReturnDeleteSuccess()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));

                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.BuildAsync();

        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三",
                Value = 1
            },
            new()
            {
                Text = "李四",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var response = await autoCompleteClient.DeleteAsync(1);
        Assert.IsTrue(response.IsValid);
    }

    [TestMethod]
    public async Task DeleteUserReturnEmpty()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三",
                Value = 1
            },
            new()
            {
                Text = "李四",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var getResponse = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        var deleteResponse = await autoCompleteClient.DeleteAsync(new[] { 1 });
        Assert.IsTrue(deleteResponse.IsValid);

        await Task.Delay(1000);

        getResponse = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 0);
    }

    [TestMethod]
    public async Task DeleteMultiUserReturnEmpty()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
                options.DefaultSearchType = SearchType.Precise;
                options.DefaultOperator = Operator.And;
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三",
                Value = 1
            },
            new()
            {
                Text = "李四",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var getResponse = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        getResponse = await autoCompleteClient.GetAsync<long>("李四");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        var deleteMultiResponse = await autoCompleteClient.DeleteAsync(new List<int>() { 1, 2, 3 });
        Assert.IsTrue(deleteMultiResponse.IsValid &&
            deleteMultiResponse.Data.Count == 3 &&
            deleteMultiResponse.Data.Count(r => r.IsValid) == 2); //todo: Masa.Utils.Data.Elasticsearch response information error

        await Task.Delay(1000);

        getResponse = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 0);
    }

    [TestMethod]
    public async Task TestAsync()
    {
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = $"index_{Guid.NewGuid()}";
                options.Alias = $"alias_{Guid.NewGuid()}";
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三:zhangsan:zhangsan@qq.com:18888888888",
                Value = 1
            },
            new()
            {
                Text = "李四:lisi:lisi@qq.com:19999999999",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var getResponse = await autoCompleteClient.GetAsync<long>("张三");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        getResponse = await autoCompleteClient.GetAsync<long>("zhangsan");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        getResponse = await autoCompleteClient.GetAsync<long>("zs");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        getResponse = await autoCompleteClient.GetAsync<long>("ls");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);

        getResponse = await autoCompleteClient.GetAsync<long>("李四");
        Assert.IsTrue(getResponse.IsValid && getResponse.Total == 1);
    }

    [TestMethod]
    public async Task TestRebuildAsync()
    {
        var indexName = $"index_{Guid.NewGuid()}";
        _services.AddAutoComplete("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(_defaultNode);
                options.ElasticsearchOptions.UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = indexName;
                options.Alias = $"alias_{Guid.NewGuid()}";
            });
        });

        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetRequiredService<IAutoCompleteClient>();
        await autoCompleteClient.BuildAsync();
        await autoCompleteClient.SetAsync(new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "张三:zhangsan:zhangsan@qq.com:18888888888",
                Value = 1
            },
            new()
            {
                Text = "李四:lisi:lisi@qq.com:19999999999",
                Value = 2
            }
        });

        await Task.Delay(1000);

        var elasticClient = ElasticClientUtils.Create(new ElasticsearchOptions(_defaultNode));
        var client = new DefaultMasaElasticClient(elasticClient);
        var countDocumentResponse = await client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.AreEqual(2, countDocumentResponse.Count);

        await autoCompleteClient.RebuildAsync();
        await Task.Delay(1000);

        countDocumentResponse = await client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.AreEqual(0, countDocumentResponse.Count);
    }
}
