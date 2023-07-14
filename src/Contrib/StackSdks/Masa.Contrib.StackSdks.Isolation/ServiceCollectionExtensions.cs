// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public static class ServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddStackIsolationAsync(this IServiceCollection services, string name)
    {
        services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiEnvironment(IsolationConsts.ENVIRONMENT);
            isolationBuilder.UseMultiEnvironment(new List<IParserProvider>()
            {
                new HttpContextItemParserProvider(),
                new QueryStringParserProvider(),
                new FormParserProvider(),
                new RouteParserProvider(),
                new HeaderParserProvider(),
                new CurrentUserEnvironmentParseProvider(),
                new MasaAppConfigureParserProvider(),
                new EnvironmentVariablesParserProvider()
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        var environments = (await pmClient.EnvironmentService.GetListAsync()).Select(e => e.Name).ToList();

        services.AddSingleton((sp) => { return new EnvironmentProvider(environments); });

        services.AddDccIsolation(builder => builder.UseDccIsolation());

        if (!name.IsNullOrEmpty())
        {
            services.ConfigureConnectionStrings(name);
        }
        services.ConfigureRedisOptions();
        services.ConfigureMultilevelCacheOptions();
        services.ConfigStorageOptions();

        services.AddScoped<EsIsolationConfigProvider>();

        services.AddScoped<EnvironmentMiddleware>();

        return services;
    }

    static void ConfigureConnectionStrings(this IServiceCollection services, string name)
    {
        var (environments, multiEnvironmentMasaStackConfig, masaStackConfig) = services.GetInternal();
        services.Configure<IsolationOptions<ConnectionStrings>>(options =>
        {
            foreach (var environment in environments)
            {
                options.Data.Add(new()
                {
                    Environment = environment,
                    Data = new ConnectionStrings(new List<KeyValuePair<string, string>>()
                    {
                        new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, multiEnvironmentMasaStackConfig.SetEnvironment(environment).GetConnectionString(name))
                    })
                });
            }
        });

        services.Configure<ConnectionStrings>(options =>
        {
            //default value map appsettings.json -> ConnectionStrings -> DefaultConnection
            if (!options.Any())
            {
                options.Add(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, masaStackConfig.GetConnectionString(name));
            }
        });
    }

    static void ConfigureRedisOptions(this IServiceCollection services)
    {
        var (environments, multiEnvironmentMasaStackConfig, masaStackConfig) = services.GetInternal();
        services.Configure<IsolationOptions<RedisConfigurationOptions>>(options =>
        {
            foreach (var environment in environments)
            {
                var redisModel = multiEnvironmentMasaStackConfig.SetEnvironment(environment).RedisModel;
                options.Data.Add(new IsolationConfigurationOptions<RedisConfigurationOptions>()
                {
                    Environment = environment,
                    Data = new RedisConfigurationOptions
                    {
                        Password = redisModel.RedisPassword,
                        Servers = new() {
                            new RedisServerOptions
                            {
                                Host = redisModel.RedisHost,
                                Port = redisModel.RedisPort
                            }
                        },
                        DefaultDatabase = redisModel.RedisDb,
                        InstanceId = environment
                    }
                });
            }
        });
        services.Configure<RedisConfigurationOptions>(options =>
        {
            var redisModel = masaStackConfig.RedisModel;
            options.Password = redisModel.RedisPassword;
            options.Servers = new()
            {
                new RedisServerOptions
                {
                    Host = redisModel.RedisHost,
                    Port = redisModel.RedisPort
                }
            };
            options.DefaultDatabase = redisModel.RedisDb;
        });
    }

    static void ConfigureMultilevelCacheOptions(this IServiceCollection services)
    {
        var (environments, _, _) = services.GetInternal();
        string subKeyPrefix = Assembly.GetEntryAssembly()?.GetName().Name ?? "";
        services.Configure<IsolationOptions<MultilevelCacheGlobalOptions>>(options =>
        {
            foreach (var environment in environments)
            {
                options.Data.Add(new IsolationConfigurationOptions<MultilevelCacheGlobalOptions>()
                {
                    Environment = environment,
                    Data = new MultilevelCacheGlobalOptions
                    {
                        InstanceId = environment,
                        SubscribeKeyType = SubscribeKeyType.SpecificPrefix,
                        SubscribeKeyPrefix = subKeyPrefix
                    }
                });
            }
        });
        services.Configure<MultilevelCacheGlobalOptions>(options =>
        {
            options.SubscribeKeyType = SubscribeKeyType.SpecificPrefix;
            options.SubscribeKeyPrefix = subKeyPrefix;
        });
    }

    static void ConfigStorageOptions(this IServiceCollection services)
    {
        var (environments, _, _) = services.GetInternal();
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<IsolationOptions<AliyunStorageConfigureOptions>>(options =>
        {
            foreach (var environment in environments)
            {
                try
                {
                    var ossOptions = configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(environment)
                        .GetSection(DccConsts.PUBLIC_ID).GetSection("$public.OSS").Get<OssOptions>();
                    if (ossOptions == null)
                    {
                        continue;
                    }
                    options.Data.Add(new IsolationConfigurationOptions<AliyunStorageConfigureOptions>()
                    {
                        Environment = environment,
                        Data = Convert(ossOptions)
                    });
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }
            }
        });

        services.Configure<AliyunStorageConfigureOptions>(options =>
        {
            var ossOptions = configuration.GetSection(SectionTypes.ConfigurationApi.ToString())
                .GetSection(DccConsts.PUBLIC_ID).GetSection("$public.OSS").Get<OssOptions>();
            options.AccessKeyId = ossOptions.AccessId;
            options.AccessKeySecret = ossOptions.AccessSecret;
            options.Sts = new AliyunStsOptions
            {
                RegionId = ossOptions.RegionId
            };
            options.Storage = new AliyunStorageOptions
            {
                BucketNames = new BucketNames(new Dictionary<string, string> { { BucketNames.DEFAULT_BUCKET_NAME, ossOptions.Bucket } }),
                Endpoint = ossOptions.Endpoint,
                RoleArn = ossOptions.RoleArn,
                RoleSessionName = ossOptions.RoleSessionName
            };
        });

        AliyunStorageConfigureOptions Convert(OssOptions ossOptions)
        {
            return new AliyunStorageConfigureOptions
            {
                AccessKeyId = ossOptions.AccessId,
                AccessKeySecret = ossOptions.AccessSecret,
                Sts = new AliyunStsOptions
                {
                    RegionId = ossOptions.RegionId
                },
                Storage = new AliyunStorageOptions
                {
                    BucketNames = new BucketNames(new Dictionary<string, string> { { BucketNames.DEFAULT_BUCKET_NAME, ossOptions.Bucket } }),
                    Endpoint = ossOptions.Endpoint,
                    RoleArn = ossOptions.RoleArn,
                    RoleSessionName = ossOptions.RoleSessionName
                }
            };
        }
    }

    public static void AddDccIsolation(this IServiceCollection services, Action<IMasaConfigurationBuilder> configureDelegate)
    {
        services.Replace(new ServiceDescriptor(typeof(IConfigurationApi), typeof(IsolationConfigurationApi), ServiceLifetime.Singleton));

        MasaConfigurationBuilder masaConfigurationBuilder = new MasaConfigurationBuilder(services, new ConfigurationBuilder());
        configureDelegate?.Invoke(masaConfigurationBuilder);
    }

    static (List<string>, IMultiEnvironmentMasaStackConfig, IMasaStackConfig) GetInternal(this IServiceCollection services)
    {
        var masaStackConfig = services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
        var multiEnvironmentMasaStackConfig = services.BuildServiceProvider().GetRequiredService<IMultiEnvironmentMasaStackConfig>();
        var environmentProvider = services.BuildServiceProvider().GetRequiredService<EnvironmentProvider>();
        return (environmentProvider.GetEnvionments(), multiEnvironmentMasaStackConfig, masaStackConfig);
    }
}
