using BmsBookTicket.Data;
using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly AppDbContext _db;

    public ShowRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Show?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Shows.FirstOrDefaultAsync(show => show.Id == id, cancellationToken);
    }

    public async Task<Show> SaveAsync(Show show, CancellationToken cancellationToken)
    {
        _db.Shows.Add(show);
        await _db.SaveChangesAsync(cancellationToken);
        return show;
    }
}
