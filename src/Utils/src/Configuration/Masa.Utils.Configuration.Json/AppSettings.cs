using Microsoft.Extensions.Configuration;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Configuration.Json
{
    public class AppSettings
    {
        private static JsonConfiguration? _config;

        public static bool Initialized => _config != null;

        public static IConfiguration Configuration
        {
            get
            {
                InitializeIfNot();

                return _config!.Configuration;
            }
        }

        private static void InitializeIfNot()
        {
            if (!Initialized)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Initialize(environment);
            }
        }

        /// <summary>
        /// Initialize configuration with appsettings.json
        /// </summary>
        public static void Initialize()
        {
            Initialize(string.Empty);
        }

        /// <summary>
        /// Initialize configuration with appsettings.json & appsettings.{environmentName}.json
        /// </summary>
        /// <param name="environmentName"></param>
        public static void Initialize(string? environmentName)
        {
            if (Initialized)
            {
                return;
            }

            var optionsList = new List<JsonFileOptions>
            {
                new JsonFileOptions
                {
                    FileName = "appsettings.json",
                    Optional = false,
                    ReloadOnChange = true
                }
            };

            if (!string.IsNullOrEmpty(environmentName))
            {
                optionsList.Add(new JsonFileOptions
                {
                    FileName = $"appsettings.{environmentName}.json",
                    Optional = true,
                    ReloadOnChange = true
                });
            }

            _config = new JsonConfiguration(optionsList);
        }

        /// <summary>
        /// Get configuration section
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onChange"></param>
        /// <returns></returns>
        public static string Get(string key, Action<string>? onChange = null)
        {
            InitializeIfNot();

            return _config!.Get(key, onChange);
        }

        /// <summary>
        /// Bind a model with configuration section
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="onChange"></param>
        /// <returns></returns>
        public static TModel GetModel<TModel>(string key, Action<TModel>? onChange = null)
        {
            InitializeIfNot();

            return _config!.GetModel(key, onChange);
        }
    }
}
