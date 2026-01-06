using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("theatres")]
public class Theatre : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

    public List<Screen> Screens { get; set; } = new();

    public int? CityId { get; set; }
    public City? City { get; set; }
}
