
namespace Masa.Contrib.BasicAbility.Pm
{
    public class PmFactory
    {
        public static IPmCaching Create(ICallerFactory callerFactory)
        {
            ArgumentNullException.ThrowIfNull(callerFactory, nameof(callerFactory));

            return new PmCaching(callerFactory.CreateClient());
        }
    }
}
