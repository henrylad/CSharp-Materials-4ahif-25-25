namespace PoolGuard.Core.Tickets;

public sealed record TicketCreationRequest(string Name, Instant ValidFrom, Instant? ValidTo);
public sealed record TicketDto(Guid Id, Instant ValidFrom, Instant ValidTo, string PersonName, double Price)
{
    public static TicketDto FromTicket(Ticket ticket) =>
        new(ticket.Id, ticket.ValidFrom, ticket.ValidTo, ticket.PersonName,ticket.Price);
}
public sealed record StatisticsDto(int CurrentVisitors, double FillLevel)
{
    public static StatisticsDto FromStatistics(ITicketService.Statistics statistics) =>
        new(statistics.CurrentVisitors, statistics.FillLevel);
}

public static class TicketEndpoint
{
    private const string ApiBasePath = "api/tickets";
    private const string GetByIdEndpointName = "GetTicketById";
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiBasePath);
        group.MapGet("{id:Guid}", (Guid id, ITicketService service) =>
        {
            var ticket = service.GetById(id);
            return ticket is not null
                ? Results.Ok(TicketDto.FromTicket(ticket))
                : Results.NotFound();
        })
        .Produces<TicketDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName(GetByIdEndpointName);
        group.MapPost("", (TicketCreationRequest request, ITicketService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest();
            }
            try
            {
                var ticket = service.CreateTicket(request.Name, request.ValidFrom, request.ValidTo);
                return Results.CreatedAtRoute(GetByIdEndpointName, new { id = ticket.Id },
                TicketDto.FromTicket(ticket));
            }
            catch (OutsideOpeningHoursException)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }
        })
    .Produces<TicketDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status400BadRequest);
        group.MapPost("{id:Guid}/entries", (Guid id, ITicketService service) =>
        {
            bool success = service.TryEnterPool(id);
            return success ? Results.NoContent() : Results.StatusCode(StatusCodes.Status403Forbidden);
        })
        .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden);
        group.MapPost("{id:Guid}/exits", (Guid id, ITicketService service) =>
    {
        bool success = service.TryExitPool(id);
        return success ? Results.NoContent() : Results.BadRequest();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);
        group.MapGet("statistics", (ITicketService service) =>
        {
            var statistics = service.GetStatistics();
            return Results.Ok(StatisticsDto.FromStatistics(statistics));
        })
        .Produces<StatisticsDto>(StatusCodes.Status200OK);
    }
}
