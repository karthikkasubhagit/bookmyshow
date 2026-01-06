using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("movies")]
public class Movie : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
