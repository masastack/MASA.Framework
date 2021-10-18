namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests.Domain.Entities
{
    public class Address : ValueObject
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        protected override IEnumerable<object> GetEqualityValues()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}
