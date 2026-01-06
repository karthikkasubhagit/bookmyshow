# BookMyShow - .NET

This is a .NET 10 Web API port of the BookMyShow ticket-booking sample.

## Tech stack
- .NET 10 (ASP.NET Core Web API)
- EF Core 10
- PostgreSQL (runtime database)
- xUnit (tests)

## Project layout
- `BmsBookTicket/` - API project
- `BmsBookTicket.Tests/` - unit tests

## Quick start
.NET 10 uses a single `Program.cs` with top-level statements. This file sets up:
- Dependency injection (services added to the container)
- Database configuration (EF Core)
- Controller endpoints

Commands:
- Restore packages: `dotnet restore`
- Build: `dotnet build`
- Run: `dotnet run`
- Test: `dotnet test`

## Configuration
The API uses PostgreSQL. Update the connection string in:
- `BmsBookTicket/appsettings.json`

Default:
```
Host=localhost;Port=5432;Database=bms;Username=postgres;Password=postgres
```

## Run the API
From `DNProjects/BmsBookTicket`:
```
dotnet run
```

The API uses HTTPS by default. The booking endpoint is:
- `POST /tickets/book`

Example request:
```
curl -X POST https://localhost:5001/tickets/book \
  -H "Content-Type: application/json" \
  -d '{"showSeatIds":[1,2],"userId":1}'
```

## Seed data
On startup, the app seeds a test user, a show, four seats, and four show seats if the database is empty. This makes the booking endpoint usable immediately.

Seed logic lives in:
- `BmsBookTicket/Data/SeedData.cs`

## Booking flow 
1. Validate user.
2. Fetch show seats (with a row-level lock in Postgres).
3. Validate seat availability.
4. Mark seats as BLOCKED.
5. Create a ticket.

## Transactions and locking
The booking flow runs inside a transaction when using a relational provider. The transaction is started in `BmsBookTicket/Services/TicketService.cs` with `IsolationLevel.Serializable` to keep the read-validate-update sequence atomic.

Row-level locking is handled in `BmsBookTicket/Repositories/ShowSeatRepository.cs`. When the provider is Postgres (Npgsql), `GetByIdsForUpdateAsync` executes a `SELECT ... FOR UPDATE` query against `show_seats` so concurrent transactions cannot lock the same rows.

Together, this ensures:
- the seat availability check is consistent,
- and two users cannot book the same seat at the same time.

## Implementation details

- `TicketController` exposes the booking endpoint and returns a `BookTicketResponseDto` in `BmsBookTicket/Controllers/TicketController.cs`.
- `TicketService` contains the core booking logic and uses a transaction when running against a relational provider in `BmsBookTicket/Services/TicketService.cs`.
- `ShowSeatRepository` applies a Postgres `FOR UPDATE` lock for seat rows when the provider is Npgsql; it falls back to a regular query for in-memory tests in `BmsBookTicket/Repositories/ShowSeatRepository.cs`.
- `ShowSeat` is the join entity between a specific `Show` and a specific `Seat`, and it stores per-show seat state in `SeatStatus`.
- Booking availability is driven by `ShowSeat.Status` so the same seat can be available for one show and blocked for another.
- Concurrency safety relies on locking `ShowSeat` rows to prevent multiple users from booking the same seat.
- EF Core configuration (features list serialization + ticket-to-seat join table) lives in `BmsBookTicket/Data/AppDbContext.cs`.
- Seed data is added on startup in `BmsBookTicket/Data/SeedData.cs` and wired in `BmsBookTicket/Program.cs`.

## Tests

Run tests from `DNProjects/BmsBookTicket.Tests`:
```
dotnet test
```

## Notes for .NET 10 beginners
- `Program.cs` is the main entry point. It configures services and the HTTP pipeline.
- Controllers live under `Controllers/` and are discovered automatically by `AddControllers()`.
- EF Core uses `DbContext` in `Data/AppDbContext.cs`.
- Dependency injection is built-in; services are registered in `Program.cs` and injected via constructors.
- JSON is handled automatically by ASP.NET Core.

If you want migrations, Swagger/OpenAPI, or a Postgres-backed test setup, ask and I can wire it up.
