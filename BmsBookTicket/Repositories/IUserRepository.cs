using BmsBookTicket.Models;

namespace BmsBookTicket.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<User> SaveAsync(User user, CancellationToken cancellationToken);
}
