using BmsBookTicket.Controllers;
using BmsBookTicket.Data;
using BmsBookTicket.Dtos;
using BmsBookTicket.Models;
using BmsBookTicket.Repositories;
using BmsBookTicket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BmsBookTicket.Tests;

public class TestTicketController
{
    private sealed class TestContext : IAsyncDisposable
    {
        public required TicketController Controller { get; init; }
        public required AppDbContext Db { get; init; }
        public required User User { get; init; }
        public required List<ShowSeat> ShowSeats { get; init; }
        public required IShowSeatRepository ShowSeatRepository { get; init; }

        public async ValueTask DisposeAsync()
        {
            await Db.DisposeAsync();
        }
    }

    private static async Task<TestContext> CreateContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"bms_{Guid.NewGuid()}")
            .Options;

        var db = new AppDbContext(options);

        var user = new User
        {
            Name = "Test User",
            Email = "test@scaler.com"
        };

        db.Users.Add(user);

        var show = new Show
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow
        };

        db.Shows.Add(show);

        var seat1 = new Seat { Name = "1A", SeatType = SeatType.Gold };
        var seat2 = new Seat { Name = "1B", SeatType = SeatType.Gold };
        var seat3 = new Seat { Name = "2A", SeatType = SeatType.Silver };
        var seat4 = new Seat { Name = "2B", SeatType = SeatType.Silver };

        db.Seats.AddRange(seat1, seat2, seat3, seat4);
        await db.SaveChangesAsync();

        var showSeat1 = new ShowSeat { Seat = seat1, Show = show, Status = SeatStatus.Available };
        var showSeat2 = new ShowSeat { Seat = seat2, Show = show, Status = SeatStatus.Available };
        var showSeat3 = new ShowSeat { Seat = seat3, Show = show, Status = SeatStatus.Available };
        var showSeat4 = new ShowSeat { Seat = seat4, Show = show, Status = SeatStatus.Available };

        db.ShowSeats.AddRange(showSeat1, showSeat2, showSeat3, showSeat4);
        await db.SaveChangesAsync();

        var showSeatRepository = new ShowSeatRepository(db);
        var ticketRepository = new TicketRepository(db);
        var userRepository = new UserRepository(db);

        var ticketService = new TicketService(db, showSeatRepository, ticketRepository, userRepository);
        var controller = new TicketController(ticketService);

        return new TestContext
        {
            Controller = controller,
            Db = db,
            User = user,
            ShowSeats = new List<ShowSeat> { showSeat1, showSeat2, showSeat3, showSeat4 },
            ShowSeatRepository = showSeatRepository
        };
    }

    [Fact]
    public async Task TestBookTicket_1_Request_Success()
    {
        await using var context = await CreateContextAsync();

        var requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[0].Id, context.ShowSeats[1].Id },
            UserId = context.User.Id
        };

        var result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseDto = Assert.IsType<BookTicketResponseDto>(okResult.Value);

        Assert.Equal(ResponseStatus.Success, responseDto.Status);
        Assert.NotNull(responseDto.Ticket);
        Assert.NotNull(responseDto.Ticket!.Seats);
        Assert.Equal(2, responseDto.Ticket.Seats.Count);

        var storedTicket = await context.Db.Tickets.FindAsync(responseDto.Ticket.Id);
        Assert.NotNull(storedTicket);

        var showSeats = await context.ShowSeatRepository.GetAllAsync(CancellationToken.None);
        var orderedShowSeats = showSeats.OrderBy(showSeat => showSeat.Id).ToList();
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[0].Status);
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[1].Status);
        Assert.Equal(SeatStatus.Available, orderedShowSeats[2].Status);
        Assert.Equal(SeatStatus.Available, orderedShowSeats[3].Status);
    }

    [Fact]
    public async Task TestBookTicket_2_Request_Success_NonOverlappingSeats()
    {
        await using var context = await CreateContextAsync();

        var requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[0].Id, context.ShowSeats[1].Id },
            UserId = context.User.Id
        };

        var result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseDto = Assert.IsType<BookTicketResponseDto>(okResult.Value);

        Assert.Equal(ResponseStatus.Success, responseDto.Status);
        Assert.NotNull(responseDto.Ticket);
        Assert.Equal(2, responseDto.Ticket!.Seats.Count);

        requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[2].Id, context.ShowSeats[3].Id },
            UserId = context.User.Id
        };

        result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        okResult = Assert.IsType<OkObjectResult>(result.Result);
        responseDto = Assert.IsType<BookTicketResponseDto>(okResult.Value);

        Assert.Equal(ResponseStatus.Success, responseDto.Status);
        Assert.NotNull(responseDto.Ticket);

        var showSeats = await context.ShowSeatRepository.GetAllAsync(CancellationToken.None);
        var orderedShowSeats = showSeats.OrderBy(showSeat => showSeat.Id).ToList();
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[0].Status);
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[1].Status);
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[2].Status);
        Assert.Equal(SeatStatus.Blocked, orderedShowSeats[3].Status);
    }

    [Fact]
    public async Task TestBookTicket_NonExistingUser_Failure()
    {
        await using var context = await CreateContextAsync();

        var requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[0].Id, context.ShowSeats[1].Id },
            UserId = 100
        };

        var result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseDto = Assert.IsType<BookTicketResponseDto>(badRequest.Value);

        Assert.Equal(ResponseStatus.Failure, responseDto.Status);
        Assert.Null(responseDto.Ticket);
    }

    [Fact]
    public async Task TestBookTicket_InvalidShowSeat_Failure()
    {
        await using var context = await CreateContextAsync();

        var requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[0].Id, 100 },
            UserId = context.User.Id
        };

        var result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseDto = Assert.IsType<BookTicketResponseDto>(badRequest.Value);

        Assert.Equal(ResponseStatus.Failure, responseDto.Status);
        Assert.Null(responseDto.Ticket);
    }

    [Fact]
    public async Task TestBookTicket_BookABookedTicket_Failure()
    {
        await using var context = await CreateContextAsync();

        var requestDto = new BookTicketRequestDto
        {
            ShowSeatIds = new List<int> { context.ShowSeats[0].Id, context.ShowSeats[1].Id },
            UserId = context.User.Id
        };

        var result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseDto = Assert.IsType<BookTicketResponseDto>(okResult.Value);

        Assert.Equal(ResponseStatus.Success, responseDto.Status);
        Assert.NotNull(responseDto.Ticket);
        Assert.Equal(2, responseDto.Ticket!.Seats.Count);

        result = await context.Controller.BookTicket(requestDto, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        responseDto = Assert.IsType<BookTicketResponseDto>(badRequest.Value);

        Assert.Equal(ResponseStatus.Failure, responseDto.Status);
        Assert.Null(responseDto.Ticket);
    }
}
