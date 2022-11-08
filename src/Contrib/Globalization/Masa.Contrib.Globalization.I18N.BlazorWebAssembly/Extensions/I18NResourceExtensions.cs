// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18NResourceExtensions
{
    private static IConfiguration? _configuration;

    private static IMasaConfiguration? _masaConfiguration;

    public static Task<I18NResource> AddJsonAsync(
        this I18NResource resource,
        string baseAddress,
        string resourcePath,
        params CultureModel[] supportedCultures)
        => resource.AddJsonAsync(baseAddress, resourcePath, supportedCultures.ToList());

    public static async Task<I18NResource> AddJsonAsync(
        this I18NResource resource,
        string baseAddress,
        string resourcePath,
        IEnumerable<CultureModel> supportedCultures)
    {
        var resourceContributors = await GetResourceContributorsAsync(
            resource,
            baseAddress,
            resourcePath,
            supportedCultures);
        foreach (var resourceContributor in resourceContributors)
        {
            resource.AddContributor(resourceContributor.CultureName, resourceContributor);
        }
        return resource;
    }

    private static async Task<List<II18NResourceContributor>> GetResourceContributorsAsync(
        I18NResource resource,
        string baseAddress,
        string resourcePath,
        IEnumerable<CultureModel> supportedCultures)
    {
        _configuration ??= MasaApp.GetServices().BuildServiceProvider().GetService<IConfiguration>();
        _masaConfiguration ??=
            MasaApp.GetServices().BuildServiceProvider().GetService<IMasaConfiguration>();

        var services = MasaApp.GetServices();
        var useMasaConfiguration = _masaConfiguration != null;
        var configuration = await AddJsonConfigurationSourceAsync(
            services,
            baseAddress,
            resourcePath,
            supportedCultures,
            resource.ResourceType,
            _configuration,
            useMasaConfiguration);
        _configuration = configuration;

        return supportedCultures.Select(supportedCulture => (II18NResourceContributor)new LocalI18NResourceContributor
                (
                    resource.ResourceType,
                    supportedCulture.Culture,
                    useMasaConfiguration ? _configuration.GetSection(SectionTypes.Local.ToString()) : _configuration!
                )
            )
            .ToList();
    }

    private static async Task<IConfiguration> AddJsonConfigurationSourceAsync(
        IServiceCollection services,
        string baseAddress,
        string resourcePath,
        IEnumerable<CultureModel> supportedCultures,
        Type resourceType,
        IConfiguration? configuration,
        bool useMasaConfiguration)
    {
        ConfigurationManager configurationManager = new();
        if (configuration == null)
        {
            configuration = configurationManager;
            services.AddSingleton<IConfiguration>(configurationManager);
        }
        else if (configuration is not ConfigurationManager)
        {
            configurationManager.AddConfiguration(configuration);
        }
        else if (configuration is ConfigurationManager configurationManagerTemp)
        {
            configurationManager = configurationManagerTemp;
        }
        var configurationBuilder = new ConfigurationBuilder();
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseAddress)
        };
        var dictionary = new Dictionary<string, Stream>();
        foreach (var culture in supportedCultures)
        {
            var memoryStream = await GetStreamAsync(httpClient, resourcePath, culture.Culture);
            if (memoryStream != null)
                dictionary.Add(culture.Culture, memoryStream);
        }
        var jsonLocalizationConfigurationSource =
            new Contrib.Globalization.I18N.BlazorWebAssembly.Json.JsonConfigurationSource(
                resourceType,
                dictionary,
                useMasaConfiguration);
        configurationBuilder.Add(jsonLocalizationConfigurationSource);
        var localizationConfiguration = configurationBuilder.Build();
        configurationManager.AddConfiguration(localizationConfiguration);
        return configuration;
    }

    private static async Task<Stream?> GetStreamAsync(HttpClient httpClient, string languageDirectory, string cultureName)
    {
        Stream? stream = null;
        try
        {
            string fileName = Path.Combine(languageDirectory, $"{cultureName}.json");
            Console.WriteLine("fileName: " + fileName);
            var jsonByteArray = await httpClient.GetByteArrayAsync(fileName);
            stream = new MemoryStream(jsonByteArray);
        }
        catch (Exception ex)
        {
            //todo:
        }
        return stream;
    }
}
