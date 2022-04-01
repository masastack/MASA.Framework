
namespace Masa.Contrib.BasicAbility.Pm
{
    public class PmCachingFactory
    {
        private static readonly Dictionary<IMemoryCacheClient, IPmCaching> _pmCachings = new();

        public static IPmCaching Create(IMemoryCacheClient client, ICallerFactory callerFactory)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (_pmCachings.TryGetValue(client, out var value))
            {
                return value;
            }

            value = new PmCaching(client, callerFactory.CreateClient());

            _pmCachings.Add(client, value);

            return value;
        }
    }
}
