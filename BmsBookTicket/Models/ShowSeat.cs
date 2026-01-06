using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("show_seats")]
public class ShowSeat : BaseModel
{
    public int? ShowId { get; set; }
    public Show? Show { get; set; }

    public int? SeatId { get; set; }
    public Seat? Seat { get; set; }

    public SeatStatus Status { get; set; }
}
