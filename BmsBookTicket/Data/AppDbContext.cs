using System.Text.Json;
using BmsBookTicket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BmsBookTicket.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Screen> Screens => Set<Screen>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Show> Shows => Set<Show>();
    public DbSet<ShowSeat> ShowSeats => Set<ShowSeat>();
    public DbSet<SeatTypeShow> SeatTypeShows => Set<SeatTypeShow>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var featureComparer = new ValueComparer<List<Feature>>(
            (left, right) => (left ?? new List<Feature>()).SequenceEqual(right ?? new List<Feature>()),
            value => (value ?? new List<Feature>()).Aggregate(0, (current, item) => HashCode.Combine(current, item.GetHashCode())),
            value => (value ?? new List<Feature>()).ToList());

        modelBuilder.Entity<Show>()
            .Property(show => show.Features)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Feature>>(v, (JsonSerializerOptions?)null) ?? new List<Feature>())
            .Metadata.SetValueComparer(featureComparer);

        modelBuilder.Entity<Screen>()
            .Property(screen => screen.Features)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Feature>>(v, (JsonSerializerOptions?)null) ?? new List<Feature>())
            .Metadata.SetValueComparer(featureComparer);

        modelBuilder.Entity<Ticket>()
            .HasMany(ticket => ticket.Seats)
            .WithMany()
            .UsingEntity(join => join.ToTable("ticket_seats"));
    }
}
