using BmsBookTicket.Data;
using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User> SaveAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
        return user;
    }
}
