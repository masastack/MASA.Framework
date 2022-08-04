// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Configuration.Json
{
    public class JsonFileOptions
    {
        /// <summary>
        /// Path relative to the base path stored in Microsoft.Extensions.Configuration.IConfigurationBuilder.Properties of builder.
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Whether the file is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        ///  Whether the configuration should be reloaded if the file changes.
        /// </summary>
        public bool ReloadOnChange { get; set; }
    }
}
