namespace BmsBookTicket.Dtos;

public class BookTicketRequestDto
{
    public List<int> ShowSeatIds { get; set; } = new();
    public int UserId { get; set; }
}
