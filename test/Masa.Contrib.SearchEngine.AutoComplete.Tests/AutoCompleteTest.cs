namespace Masa.Contrib.SearchEngine.AutoComplete.Tests;

[TestClass]
public class AutoCompleteTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void TestAddAutoCompleteAndNoIndexName()
    {
        var builder = _services.AddElasticsearchClient();
        Assert.ThrowsException<ArgumentNullException>(() => builder.AddAutoComplete());
    }

    [TestMethod]
    public void TestAddAutoComplete()
    {
        var builder = _services.AddElasticsearchClient("es", option =>
        {
            option.UseConnectionSettings(setting =>
            {
                setting.DefaultIndex("user_index");
            });
        }).AddAutoComplete();
        var serviceProvider = _services.BuildServiceProvider();
        var autoCompleteClient = serviceProvider.GetService<IAutoCompleteClient>();
        Assert.IsNotNull(autoCompleteClient);

        var autoCompleteFactory = serviceProvider.GetService<IAutoCompleteFactory>();
        Assert.IsNotNull(autoCompleteFactory);

        var autoCompleteClient2 = autoCompleteFactory.CreateClient();
        Assert.IsNotNull(autoCompleteClient2);

        var elasticClientField = typeof(AutoCompleteClient).GetField("_elasticClient", BindingFlags.Instance | BindingFlags.NonPublic)!;
        Assert.IsTrue(elasticClientField.GetValue(autoCompleteClient) == elasticClientField.GetValue(autoCompleteClient2));
    }

    [TestMethod]
    public void TestAddMultiAutoComplete()
    {
        string userIndexName = "user_index_01";
        string userAlias = "user_index";

        var builder = _services
            .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
            .AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
        Assert.ThrowsException<ArgumentException>(() => builder.AddAutoComplete(option => option.UseIndexName(userIndexName)));
    }

    [TestMethod]
    public void TestAddMultiDefaultAutoComplete()
    {
        string userIndexName = "user_index_01";
        string userAlias = "user_index";

        var builder = _services
            .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
            .AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias).UseDefault());
        Assert.ThrowsException<ArgumentException>(()
            => builder.AddAutoComplete(option => option.UseIndexName("employee_index").UseDefault()));
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        string userIndexName = "user_index_01";
        string userAlias = "user_index";

        var builder = _services
            .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault());

        await builder.Client.DeleteIndexByAliasAsync(userAlias);

        builder.AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias).UseDefaultSearchType(SearchType.Fuzzy));

        var autoCompleteFactory = builder.Services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient(userIndexName);
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
            },
            new()
            {
                Text = "张丽",
                Value = 3
            }
        });

        Thread.Sleep(1000);

        var response = await autoCompleteClient.GetAsync<long>("张三", new AutoCompleteOptions(SearchType.Precise));
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("三", new AutoCompleteOptions(SearchType.Precise));
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zs", new AutoCompleteOptions(SearchType.Precise));
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zhang", new AutoCompleteOptions(SearchType.Precise));
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("li", new AutoCompleteOptions(SearchType.Precise));
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("*si");
        Assert.IsTrue(response.IsValid && response.Total == 1);

        response = await autoCompleteClient.GetAsync<long>("zhang*");
        Assert.IsTrue(response.IsValid && response.Total == 2);

        response = await autoCompleteClient.GetAsync<long>("*");
        Assert.IsTrue(response.IsValid && response.Total == 3);
    }

    [TestMethod]
    public async Task TestCustomModelAsync()
    {
        string employeeIndexName = "employee_index_01";
        string employeeAlias = "employee_index";
        var builder = _services.AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200"));

        await builder.Client.DeleteIndexByAliasAsync(employeeAlias);

        string analyzer = "ik_max_word_pinyin";
        builder.AddAutoComplete<Employee, int>(option => option
            .UseIndexName(employeeIndexName)
            .UseAlias(employeeAlias)
            .Mapping(descriptor =>
            {
                descriptor.AutoMap<Employee>()
                    .Properties(ps =>
                        ps.Text(s =>
                            s.Name(n => n.Id)
                                .Analyzer(analyzer)
                        )
                    )
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
                    );
            }));

        var autoCompleteFactory = builder.Services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var employeeClient = autoCompleteFactory.CreateClient(employeeAlias);
        await employeeClient.SetAsync(new Employee[]
        {
            new()
            {
                Text = "吉姆",
                Value = 1,
                Phone = "13999999999"
            },
            new()
            {
                Text = "托尼",
                Value = 2,
                Phone = "13888888888"
            }
        });

        Thread.Sleep(1000);

        var employeeResponse = await employeeClient.GetAsync<Employee, int>("吉姆");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("139");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 0);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("139*", new AutoCompleteOptions()
        {
            Field = "phone"
        });
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("*");
        Assert.IsTrue(employeeResponse.Data.Any(employee => employee.Phone == "13999999999"));

        await employeeClient.SetAsync(new Employee[]
        {
            new()
            {
                Text = "吉姆",
                Value = 1,
                Phone = "13777777777"
            }
        });

        Thread.Sleep(1000);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("*");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 2);
        Assert.IsTrue(employeeResponse.Data.Any(employee => employee.Phone == "13777777777"));

        await employeeClient.SetAsync(new Employee[]
        {
            new()
            {
                Text = "吉姆",
                Value = 1,
                Phone = "13999999999"
            }
        }, new SetOptions()
        {
            IsOverride = false
        });

        Thread.Sleep(1000);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("*");
        Assert.IsTrue(employeeResponse.Data.Any(employee => employee.Phone == "13777777777"));
    }

    [TestMethod]
    public async Task TestPreciseAsync()
    {
        string employeeIndexName = "employee_index_01";
        string employeeAlias = "employee_index";
        var builder = _services.AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200"));

        await builder.Client.DeleteIndexByAliasAsync(employeeAlias);
        string analyzer = "ik_max_word_pinyin";
        builder.AddAutoComplete<Employee, int>(option => option
            .UseIndexName(employeeIndexName)
            .UseAlias(employeeAlias)
            .Mapping(descriptor =>
            {
                descriptor.AutoMap<Employee>()
                    .Properties(ps =>
                        ps.Text(s =>
                            s.Name(n => n.Id)
                                .Analyzer(analyzer)
                        )
                    )
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
                    );
            }));

        var autoCompleteFactory = builder.Services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var employeeClient = autoCompleteFactory.CreateClient(employeeAlias);
        await employeeClient.SetAsync(new Employee[]
        {
            new()
            {
                Text = "吉姆",
                Value = 1,
                Phone = "13999999999"
            },
            new()
            {
                Text = "托尼",
                Value = 2,
                Phone = "13888888888"
            }
        });

        Thread.Sleep(1000);

        var employeeResponse = await employeeClient.GetAsync<Employee, int>("ji*");
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1);

        employeeResponse = await employeeClient.GetAsync<Employee, int>("13999999999", new AutoCompleteOptions(SearchType.Precise)
        {
            Field = "phone"
        });
        Assert.IsTrue(employeeResponse.IsValid && employeeResponse.Total == 1 &&
            employeeResponse.Data.Any(employee => employee.Value == 1));
    }

    [TestMethod]
    public async Task TestOperatorAndAsync()
    {
        string userIndexName = "user_index_01";
        string userAlias = "user_index";

        var builder = _services
            .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
            .AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias).UseDefaultOperator(Nest.Operator.And));

        await builder.Client.ClearDocumentAsync(userAlias);

        var autoCompleteFactory = builder.Services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient(userAlias);
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
                Value = 1
            }
        });

        Thread.Sleep(1000);

        var response = await autoCompleteClient.GetAsync<long>("Edward Adam Davis");
        Assert.IsTrue(response.IsValid && response.Total == 1);
    }

    [TestMethod]
    public async Task TestOperatorOrAsync()
    {
        string userIndexName = "user_index_01";
        string userAlias = "user_index";

        var builder = _services
            .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault());

        await builder.Client.DeleteIndexAsync(userIndexName);

        builder.AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias).IndexSettings(setting =>
        {
            string analyzer = "ik_max_word_pinyin";
            IAnalysis analysis = new AnalysisDescriptor();
            analysis.Analyzers = new Analyzers();
            analysis.TokenFilters = new TokenFilters();
            IIndexSettings indexSettings = new IndexSettings()
            {
                Analysis = analysis
            };
            string defaultAnalyzer = "ik_max_word";
            string pinyinFilter = "pinyin";
            string wordDelimiterFilter = "word_delimiter";
            indexSettings.Analysis.Analyzers.Add(analyzer, new CustomAnalyzer()
            {
                Filter = new[] { pinyinFilter, wordDelimiterFilter },
                Tokenizer = defaultAnalyzer
            });
            indexSettings.Analysis.TokenFilters.Add(pinyinFilter, new PinYinTokenFilterDescriptor());
        }));

        var autoCompleteFactory = builder.Services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.CreateClient(userAlias);
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
                Value = 1
            }
        });

        Thread.Sleep(1000);

        var response = await autoCompleteClient.GetAsync<long>("Edward Adam Davis");
        Assert.IsTrue(response.IsValid && response.Total == 2);
    }

    [TestMethod]
    public void TestNullIndexName()
    {
        var builder = _services.AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200"));

        string analyzer = "ik_max_word_pinyin";

        Assert.ThrowsException<ArgumentNullException>(() => builder.AddAutoComplete<Employee, int>(option => option
            .Mapping(descriptor =>
            {
                descriptor.AutoMap<Employee>()
                    .Properties(ps =>
                        ps.Text(s =>
                            s.Name(n => n.Id)
                                .Analyzer(analyzer)
                        )
                    )
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
                    );
            })));
    }
}
