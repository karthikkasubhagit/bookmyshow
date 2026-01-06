using System.ComponentModel.DataAnnotations.Schema;

namespace BmsBookTicket.Models;

[Table("users")]
public class User : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
}
