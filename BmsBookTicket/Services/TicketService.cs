using System.Data;
using BmsBookTicket.Models;
using BmsBookTicket.Repositories;
using BmsBookTicket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BmsBookTicket.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;
    private readonly IShowSeatRepository _showSeatRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public TicketService(
        AppDbContext db,
        IShowSeatRepository showSeatRepository,
        ITicketRepository ticketRepository,
        IUserRepository userRepository)
    {
        _db = db;
        _showSeatRepository = showSeatRepository;
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<Ticket> BookTicketAsync(List<int> showSeatIds, int userId, CancellationToken cancellationToken)
    {
        IDbContextTransaction? transaction = null;
        try
        {
            if (_db.Database.IsRelational())
            {
                transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            }

            var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new Exception("Invalid user");
            }

            var showSeats = await _showSeatRepository.GetByIdsForUpdateAsync(showSeatIds, cancellationToken);
            if (showSeats.Count != showSeatIds.Count)
            {
                throw new Exception("Invalid show seats");
            }

            var show = showSeats.Select(showSeat => showSeat.Show).FirstOrDefault();
            if (show is null)
            {
                throw new Exception("Invalid show");
            }

            var bookedSeats = showSeats.Where(showSeat => showSeat.Status != SeatStatus.Available).ToList();
            if (bookedSeats.Count > 0)
            {
                throw new Exception("Seats are not available");
            }

            foreach (var showSeat in showSeats)
            {
                showSeat.Status = SeatStatus.Blocked;
            }

            await _showSeatRepository.SaveAllAsync(showSeats, cancellationToken);

            var ticket = new Ticket
            {
                Seats = showSeats.Select(showSeat => showSeat.Seat).Where(seat => seat is not null).Cast<Seat>().ToList(),
                Show = show,
                TimeOfBooking = DateTime.UtcNow,
                User = user,
                Status = TicketStatus.Unpaid
            };

            var savedTicket = await _ticketRepository.SaveAsync(ticket, cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return savedTicket;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
