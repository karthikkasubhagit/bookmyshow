using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("shows")]
public class Show : BaseModel
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public List<Feature> Features { get; set; } = new();

    public int? ScreenId { get; set; }
    public Screen? Screen { get; set; }
}
