namespace Masa.Contrib.Isolation.MultiTenant;

public class MultiTenantOptions
{
    public string TenantKey { get; set; }

    public List<IParserProvider> ParserProviders { get; set; }
}
