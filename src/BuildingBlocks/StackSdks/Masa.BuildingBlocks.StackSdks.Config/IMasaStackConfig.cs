// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Config.Models;

namespace Masa.BuildingBlocks.StackSdks.Config
{
    public interface IMasaStackConfig
    {
        string GetValue(string key);

        void SetValue(string key, string value);

        public RedisModel? RedisModel { get; }

        public string IsDemo { get; }

        public string TlsName { get; }

        public List<string> ProjectList { get; }

        public string Version { get; }

        public string Cluster { get; }

        public string OtlpUrl { get; }
    }
}
