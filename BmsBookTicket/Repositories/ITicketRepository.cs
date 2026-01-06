using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public interface ITicketRepository
{
    Task<Ticket> SaveAsync(Ticket ticket, CancellationToken cancellationToken);
}
