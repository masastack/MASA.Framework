namespace Masa.Contrib.Isolation.MultiTenant;

public class ConvertProvider : IConvertProvider
{
    public object ChangeType(string value, Type conversionType)
    {
        var result = conversionType == typeof(Guid) ? Guid.Parse(value) : Convert.ChangeType(value, conversionType);
        return result;
    }
}
