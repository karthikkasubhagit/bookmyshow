using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("screens")]
public class Screen : BaseModel
{
    public string Name { get; set; } = string.Empty;

    public List<Seat> Seats { get; set; } = new();

    public ScreenStatus Status { get; set; }

    public List<Feature> Features { get; set; } = new();

    public int? TheatreId { get; set; }
    public Theatre? Theatre { get; set; }
}
