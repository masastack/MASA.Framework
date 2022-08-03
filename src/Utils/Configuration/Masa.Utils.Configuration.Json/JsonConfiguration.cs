using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Configuration.Json
{
    public class JsonConfiguration
    {
        public IConfiguration Configuration { get; }

        public JsonConfiguration(string fileName)
            : this(Directory.GetCurrentDirectory(), fileName)
        {
        }

        public JsonConfiguration(string basePath, string fileName)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(fileName, optional: false, reloadOnChange: true)
                .Build();
        }

        public JsonConfiguration(JsonFileOptions options)
            : this(Directory.GetCurrentDirectory(), options)
        {
        }

        public JsonConfiguration(string basePath, JsonFileOptions options)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(options.FileName, options.Optional, options.ReloadOnChange)
                .Build();
        }

        public JsonConfiguration(List<JsonFileOptions> optionsList)
            : this(Directory.GetCurrentDirectory(), optionsList)
        {
        }

        public JsonConfiguration(string basePath, List<JsonFileOptions> optionsList)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath);

            foreach (var options in optionsList)
            {
                builder.AddJsonFile(options.FileName, options.Optional, options.ReloadOnChange);
            }

            Configuration = builder.Build();
        }

        /// <summary>
        /// Get configuration section
        /// </summary>
        public string Get(string key, Action<string>? onChange = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var model = Get(key);

            if (onChange != null)
            {
                ChangeToken.OnChange(() => Configuration.GetReloadToken(), () =>
                {
                    onChange(Get(key));
                });
            }

            return model;
        }

        /// <summary>
        /// Bind a model with configuration section
        /// </summary>
        public TModel GetModel<TModel>(string key, Action<TModel>? onChange = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var model = Get<TModel>(key);

            if (onChange != null)
            {
                ChangeToken.OnChange(() => Configuration.GetReloadToken(), () =>
                {
                    onChange(Get<TModel>(key));
                });
            }

            return model;
        }

        private string Get(string key)
        {
            var options = Configuration.GetSection(key).Value;
            return options;
        }

        /// <summary>
        /// Bind a model with configuration section
        /// </summary>
        private TModel Get<TModel>(string key)
        {
            var options = Configuration.GetSection(key).Get<TModel>();
            return options;
        }
    }
}
