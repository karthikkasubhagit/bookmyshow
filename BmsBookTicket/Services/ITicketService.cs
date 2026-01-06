using BmsBookTicket.Models;

namespace BmsBookTicket.Services;

public interface ITicketService
{
    Task<Ticket> BookTicketAsync(List<int> showSeatIds, int userId, CancellationToken cancellationToken);
}
