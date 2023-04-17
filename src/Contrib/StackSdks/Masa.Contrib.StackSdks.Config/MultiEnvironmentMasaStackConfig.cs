// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config
{
    public class MultiEnvironmentMasaStackConfig : MasaStackConfig, IMultiEnvironmentMasaStackConfig
    {
        private readonly IConfigurationApiClient _configurationApiClient;

        private static ConcurrentDictionary<string, string> ConfigMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public MultiEnvironmentMasaStackConfig(IConfigurationApiClient client, Dictionary<string, string> configs)
        {
            _configurationApiClient = client;
            ConfigMap = new(configs);
        }

        public override Dictionary<string, string> GetValues()
        {
            try
            {
                var remoteConfigs = _configurationApiClient.GetAsync<Dictionary<string, string>>(
                   ConfigMap[MasaStackConfigConstant.ENVIRONMENT],
                   ConfigMap[MasaStackConfigConstant.CLUSTER],
                   DEFAULT_PUBLIC_ID,
                   DEFAULT_CONFIG_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
                return remoteConfigs;
            }
            catch (ArgumentException)
            {
                return new(ConfigMap);
            }
        }

        public IMasaStackConfig SetEnvironment(string environment)
        {
            var configs = new Dictionary<string, string>(ConfigMap)
            {
                [MasaStackConfigConstant.ENVIRONMENT] = environment
            };

            var stackConfig = new MultiEnvironmentMasaStackConfig(_configurationApiClient, configs);
            return stackConfig;
        }
    }
}
