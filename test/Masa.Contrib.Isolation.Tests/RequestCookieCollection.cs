namespace Masa.Contrib.Isolation.Tests;

public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
{
    public new ICollection<string> Keys { get; }
}
