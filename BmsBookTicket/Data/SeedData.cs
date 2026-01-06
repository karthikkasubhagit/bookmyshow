using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.ShowSeats.AnyAsync())
        {
            return;
        }

        var user = new User
        {
            Name = "Test User",
            Email = "test@scaler.com"
        };

        var show = new Show
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow
        };

        var seat1 = new Seat { Name = "1A", SeatType = SeatType.Gold };
        var seat2 = new Seat { Name = "1B", SeatType = SeatType.Gold };
        var seat3 = new Seat { Name = "2A", SeatType = SeatType.Silver };
        var seat4 = new Seat { Name = "2B", SeatType = SeatType.Silver };

        var showSeat1 = new ShowSeat { Seat = seat1, Show = show, Status = SeatStatus.Available };
        var showSeat2 = new ShowSeat { Seat = seat2, Show = show, Status = SeatStatus.Available };
        var showSeat3 = new ShowSeat { Seat = seat3, Show = show, Status = SeatStatus.Available };
        var showSeat4 = new ShowSeat { Seat = seat4, Show = show, Status = SeatStatus.Available };

        db.Users.Add(user);
        db.Shows.Add(show);
        db.Seats.AddRange(seat1, seat2, seat3, seat4);
        db.ShowSeats.AddRange(showSeat1, showSeat2, showSeat3, showSeat4);

        await db.SaveChangesAsync();
    }
}
