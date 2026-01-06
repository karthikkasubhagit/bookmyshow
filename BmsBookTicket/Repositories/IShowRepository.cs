using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public interface IShowRepository
{
    Task<Show?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<Show> SaveAsync(Show show, CancellationToken cancellationToken);
}
