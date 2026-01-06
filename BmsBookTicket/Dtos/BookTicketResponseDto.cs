using BmsBookTicket.Models;

namespace BmsBookTicket.Dtos;

public class BookTicketResponseDto
{
    public ResponseStatus Status { get; set; }
    public Ticket? Ticket { get; set; }
}
