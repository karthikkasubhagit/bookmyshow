using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("seats")]
public class Seat : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public SeatType SeatType { get; set; }

    public int? ScreenId { get; set; }
    public Screen? Screen { get; set; }
}
