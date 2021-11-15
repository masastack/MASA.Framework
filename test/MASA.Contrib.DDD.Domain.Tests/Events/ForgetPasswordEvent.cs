namespace MASA.Contrib.DDD.Domain.Tests.Events;

public class ForgetPasswordEvent : IEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    public string Account { get; set; }
}
