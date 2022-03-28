namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal.Options;

internal class Tables
{
    public string Name { get; }

    public string Schema { get; }

    public List<(string Name, string ColunName)> Properties { get; }

    public Tables(string name, string schema, List<(string Name, string ColunName)> properties)
    {
        Name = name;
        Schema = schema;
        Properties = properties;
    }
}
