using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public interface ISeatRepository
{
    Task<List<Seat>> FindAllByIdAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<Seat> SaveAsync(Seat seat, CancellationToken cancellationToken);
}
