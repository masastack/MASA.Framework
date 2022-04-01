namespace Masa.Contrib.Isolation.MultiTenant;

public class ConvertProvider : IConvertProvider
{
    public object ChangeType(string value, Type conversionType)
    {
        object result;
        if (conversionType == typeof(Guid))
        {
            result = Guid.Parse(value);
        }
        else
        {
            result = Convert.ChangeType(value, conversionType);
        }
        return result;
    }
}
