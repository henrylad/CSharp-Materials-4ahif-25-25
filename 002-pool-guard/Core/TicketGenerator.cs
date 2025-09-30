using NodaTime.Extensions;

namespace PoolGuard.Core;

public interface ITicketGenerator
{
    public Ticket GenerateTicket(string personName, Instant validFrom);
}

public class TicketGenerator(IClock clock) : ITicketGenerator
{


    public Ticket GenerateTicket(string personName, Instant validFrom)
    {
        var validDuration = GetTimeUntilClosing();
        var validTo = validFrom.Plus(validDuration);

        return new Ticket
        {
            Id = Guid.CreateVersion7(),
            PersonName = personName,
            ValidFrom = validFrom,
            ValidTo = validTo
        };

        
    }

    private Duration GetTimeUntilClosing()
    {
        var currentTime = clock.GetCurrentInstant().ToLocalDateTime().TimeOfDay;
        var diff = Period.Between(currentTime, Const.ClosingTime).ToDuration();

        return diff < Duration.Zero ? Duration.Zero : diff;
    }
}