using BmsBookTicket.Data;
using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Repositories;

public class ShowSeatRepository : IShowSeatRepository
{
    private readonly AppDbContext _db;

    public ShowSeatRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ShowSeat>> GetByIdsForUpdateAsync(List<int> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return new List<ShowSeat>();
        }

        if (_db.Database.IsRelational() && _db.Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            return await _db.ShowSeats
                .FromSqlInterpolated($"SELECT * FROM show_seats WHERE id = ANY({ids}) FOR UPDATE")
                .Include(showSeat => showSeat.Show)
                .Include(showSeat => showSeat.Seat)
                .ToListAsync(cancellationToken);
        }

        return await _db.ShowSeats
            .Include(showSeat => showSeat.Show)
            .Include(showSeat => showSeat.Seat)
            .Where(showSeat => ids.Contains(showSeat.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveAllAsync(IEnumerable<ShowSeat> showSeats, CancellationToken cancellationToken)
    {
        _db.ShowSeats.UpdateRange(showSeats);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ShowSeat>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _db.ShowSeats
            .Include(showSeat => showSeat.Show)
            .Include(showSeat => showSeat.Seat)
            .ToListAsync(cancellationToken);
    }
}
