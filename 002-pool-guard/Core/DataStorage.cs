namespace PoolGuard.Core.Tickets;

public interface IDataStorage
{
    public Ticket? GetTicket(Guid ticketId);
    public void SaveTicket(Ticket ticket);
    public IReadOnlyCollection<Ticket> GetAllTickets();
}

public sealed class DataStorage : IDataStorage
{
    private readonly Dictionary<Guid, Ticket> _tickets = new();
    public Ticket? GetTicket(Guid ticketId) => _tickets.GetValueOrDefault(ticketId);
    public void SaveTicket(Ticket ticket)
    {
        _tickets[ticket.Id] = ticket;
    }
    public IReadOnlyCollection<Ticket> GetAllTickets() => _tickets.Values;
}