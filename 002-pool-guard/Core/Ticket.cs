namespace PoolGuard.Core;


public sealed class Ticket
{
    private readonly List<AccessEvent> _accessEvents = [];
    public Guid Id { get; init; }

    public Instant ValidFrom { get; init; }
    public Instant ValidTo { get; init; }
    public required string PersonName { get; init; }

    public bool IsInPoolArea => _accessEvents[^1].Type is AccessTypeEvent.Enter;

    public bool AddAccessEvent(AccessTypeEvent type, Instant timestamp)
    {
        if (timestamp < ValidFrom || timestamp > ValidTo)
        {
            return false;
        }

        switch (type)
        {
            case AccessTypeEvent.Enter when IsInPoolArea:
            case AccessTypeEvent.Exit when !IsInPoolArea:
                { return false; }
            case AccessTypeEvent.Enter or AccessTypeEvent.Exit:
                {
                    _accessEvents.Add(new AccessEvent(timestamp, type));
                    return true;
                }
            default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Wrong access type provided for access event ");
                }
        }

    }
}


public readonly record struct AccessEvent(Instant Timestamp, AccessTypeEvent Type);


public enum AccessTypeEvent
{
    Enter = 10,
    Exit = 20
}