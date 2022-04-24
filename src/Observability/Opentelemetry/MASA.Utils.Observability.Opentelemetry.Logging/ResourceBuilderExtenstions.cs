namespace OpenTelemetry.Resources
{
    public static class ResourceBuilderExtenstions
    {
        public static ResourceBuilder AddMasaService(
           this ResourceBuilder resourceBuilder,
           string serviceName,
           string serviceProjectId,
           string serviceNamespace = null,
           string serviceVersion = null,
           bool autoGenerateServiceInstanceId = true,
           string serviceInstanceId = null)
        {
            resourceBuilder = resourceBuilder.AddService(serviceName, serviceNamespace, serviceVersion, autoGenerateServiceInstanceId, serviceInstanceId);

            if (!string.IsNullOrEmpty(serviceProjectId))
            {
                resourceBuilder.AddAttributes(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(MasaResourceSemanticConventions.AttributeServiceProjectId, serviceProjectId) });
            }
            return resourceBuilder;
        }
    }
}
