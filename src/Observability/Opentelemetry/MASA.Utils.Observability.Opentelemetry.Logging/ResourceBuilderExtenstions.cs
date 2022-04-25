using Masa.Contrib.Observability.Opentelemetry.Logging;

namespace OpenTelemetry.Resources
{
    public static class ResourceBuilderExtenstions
    {
        public static ResourceBuilder AddMasaService(
           this ResourceBuilder resourceBuilder,
           MasaOpenTelemetryLogOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException("Must not be null or empty", nameof(options));
            }
           
             resourceBuilder = resourceBuilder.AddService(options.ServiceName, options.ServiceNameSpace, options.ServerVersion, true, options.ServiceInstanceId);

            if (!string.IsNullOrEmpty(options.ProjectName))
            {
                resourceBuilder.AddAttributes(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(MasaResourceSemanticConventions.AttributeServiceProjectId, options.ProjectName) });
            }
            return resourceBuilder;
        }
    }
}
