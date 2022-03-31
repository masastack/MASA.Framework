namespace Masa.Contrib.Isolation.UoW.EF.Tests;

public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
{
    public new ICollection<string> Keys { get; }
}
