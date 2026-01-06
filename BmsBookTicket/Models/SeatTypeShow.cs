using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("seat_type_shows")]
public class SeatTypeShow : BaseModel
{
    public int? ShowId { get; set; }
    public Show? Show { get; set; }

    public SeatType SeatType { get; set; }
    public double Price { get; set; }
}
