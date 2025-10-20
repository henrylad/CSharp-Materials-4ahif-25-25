namespace PoolGuard.Core.Tickets;

public interface ITicketService
{
    public Ticket? GetById(Guid ticketId);
    public Ticket CreateTicket(string personName, Instant? validFrom = null, Instant? validTo = null);
    public bool TryEnterPool(Guid ticketId);
    public bool TryExitPool(Guid ticketId);
    public Statistics GetStatistics();

    public readonly record struct Statistics(int CurrentVisitors, double FillLevel);
}

public sealed class TicketService(
    IClock clock,
    ITicketGenerator ticketGenerator,
    IDataStorage dataStorage) : ITicketService
{

    public Ticket? GetById(Guid ticketId) => dataStorage.GetTicket(ticketId);

    public Ticket CreateTicket(string personName, Instant? validFrom = null, Instant? validTo = null)
    {
        var now = clock.GetCurrentInstant();
        var currentTime = now.ToLocalDateTime().TimeOfDay;
        if (currentTime < Const.OpeningTime || currentTime > Const.ClosingTime)
        {
            throw new OutsideOpeningHoursException();
        }
        var ticket = ticketGenerator.GenerateTicket(personName, validFrom ?? now, validTo);
        dataStorage.SaveTicket(ticket);
        return ticket;
    }

    public bool TryEnterPool(Guid ticketId) => TryAddAccessEvent(ticketId,
    AccessTypeEvent.Enter);
    public bool TryExitPool(Guid ticketId) => TryAddAccessEvent(ticketId,
    AccessTypeEvent.Exit);


    public ITicketService.Statistics GetStatistics()
    {
        int currentVisitors = GetCurrentVisitors();
        double fillLevel = Math.Round(currentVisitors / (double) Const.MaxCapacity,
        2);
        return new ITicketService.Statistics(currentVisitors, fillLevel);
    }

    private bool TryAddAccessEvent(Guid ticketId, AccessTypeEvent accessEvent)
    {
        if (accessEvent is AccessTypeEvent.Enter)
        {
            if (GetCurrentVisitors() >= Const.MaxCapacity)
            {
                return false; // ⑨ capacity check
            }
        }

        var now = clock.GetCurrentInstant();
        var ticket = dataStorage.GetTicket(ticketId);
        return ticket?.AddAccessEvent(accessEvent, now) ?? false;
    }

    private int GetCurrentVisitors() =>
        dataStorage.GetAllTickets().Count(ticket => ticket.IsInPoolArea);

}


public sealed class OutsideOpeningHoursException() : Exception("The pool is currently closed.");