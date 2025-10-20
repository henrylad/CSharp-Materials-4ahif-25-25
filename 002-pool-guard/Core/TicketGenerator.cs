using NodaTime;
using NodaTime.Extensions;

namespace PoolGuard.Core;

public interface ITicketGenerator
{
    public Ticket GenerateTicket(string personName, Instant validFrom, Instant? validTo = null);
}

public class TicketGenerator(IClock clock) : ITicketGenerator
{


    public Ticket GenerateTicket(string personName, Instant validFrom, Instant? validTo = null)
    {
        var validDuration = GetTimeUntilClosing(validFrom, validTo);
        var effectiveValidFrom = validFrom == default ? clock.GetCurrentInstant() : validFrom;
        validTo ??= effectiveValidFrom.Plus(validDuration);

        return new Ticket
        {
            Id = Guid.CreateVersion7(),
            PersonName = personName,
            ValidFrom = effectiveValidFrom,
            ValidTo = validTo ?? effectiveValidFrom.Plus(validDuration),
            Price = GetPrice(validDuration.TotalHours)
        };
    }

    private Duration GetTimeUntilClosing(Instant? from = null, Instant? to = null)
    {
        var currentTime = from ?? clock.GetCurrentInstant();
        var closingTime = to ?? currentTime;
        var diff = closingTime - currentTime;

        return diff < Duration.Zero ? Duration.Zero : diff;
    }

    private double GetPrice(double totalHours)
    {
        const double DayPrice = 10;
        const double HourPrice = 1.8;

        var totalDayPrice = Math.Floor(totalHours / 24) * DayPrice;
        var totalHourPrice = (totalHours % 24) <= Period.Between(Const.OpeningTime, Const.ClosingTime).Hours ? (totalHours % 24) * HourPrice : throw new Exception($"Maximum daily usage is {Period.Between(Const.OpeningTime, Const.ClosingTime).Hours} hours.");
        var totalPrice = totalDayPrice + totalHourPrice;

        Console.WriteLine($"Total days: {totalDayPrice}");
        Console.WriteLine($"Total hours: {totalHourPrice}");
        Console.WriteLine($"Total price: {totalPrice}");

        return Math.Round(totalPrice, 2);
    }
}