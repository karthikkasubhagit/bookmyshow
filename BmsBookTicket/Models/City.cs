using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("cities")]
public class City : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public List<Theatre> Theatres { get; set; } = new();
}
