namespace OpenTelemetry.Resources;

public static class ResourceBuilderExtenstions
{
    public static ResourceBuilder AddMasaService(
       this ResourceBuilder resourceBuilder,
       MasaLoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(resourceBuilder, nameof(resourceBuilder));

        resourceBuilder = resourceBuilder.AddService(options.ServiceName, options.ServiceNameSpace, options.ServerVersion, true, options.ServiceInstanceId);

        if (!string.IsNullOrEmpty(options.ProjectName))
        {
            resourceBuilder.AddAttributes(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(MasaResourceSemanticConventions.AttributeServiceProjectName, options.ProjectName) });
        }
        return resourceBuilder;
    }
}
