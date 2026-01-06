using BmsBookTicket.Data;
using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly AppDbContext _db;

    public SeatRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Seat>> FindAllByIdAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();
        return await _db.Seats.Where(seat => idList.Contains(seat.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Seat> SaveAsync(Seat seat, CancellationToken cancellationToken)
    {
        _db.Seats.Add(seat);
        await _db.SaveChangesAsync(cancellationToken);
        return seat;
    }
}
