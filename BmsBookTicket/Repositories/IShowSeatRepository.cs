using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public interface IShowSeatRepository
{
    Task<List<ShowSeat>> GetByIdsForUpdateAsync(List<int> ids, CancellationToken cancellationToken);
    Task SaveAllAsync(IEnumerable<ShowSeat> showSeats, CancellationToken cancellationToken);
    Task<List<ShowSeat>> GetAllAsync(CancellationToken cancellationToken);
}
