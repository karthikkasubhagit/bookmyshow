using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("tickets")]
public class Ticket : BaseModel
{
    public int? ShowId { get; set; }
    public Show? Show { get; set; }

    public List<Seat> Seats { get; set; } = new();

    public DateTime TimeOfBooking { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public TicketStatus Status { get; set; }
}
