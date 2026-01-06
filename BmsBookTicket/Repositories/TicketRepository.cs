using BmsBookTicket.Data;
using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Ticket> SaveAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);
        return ticket;
    }
}
