namespace Masa.Contrib.Ddd.Domain.Repository.EF.CombinedKeys.Tests;

public class Students : AggregateRoot
{
    public Students()
    {
        RegisterTime = DateTime.UtcNow;
    }

    public string SerialNumber { get; set; } = default!;

    public string Name { get; set; }

    public int Age { get; set; }

    public DateTime RegisterTime { get; private set; }

    /// <summary>
    /// Test the case of the joint primary key error, no business value
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<(string Name, object Value)> GetKeys()
        => new List<(string Name, object Value)>()
        {
            ("SerialNumber", SerialNumber),
            ("","")
        };
}
