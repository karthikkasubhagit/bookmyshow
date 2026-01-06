using BmsBookTicket.Dtos;
using BmsBookTicket.Services;
using Microsoft.AspNetCore.Mvc;

namespace BmsBookTicket.Controllers;

[ApiController]
[Route("tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost("book")]
    public async Task<ActionResult<BookTicketResponseDto>> BookTicket([FromBody] BookTicketRequestDto requestDto, CancellationToken cancellationToken)
    {
        var response = new BookTicketResponseDto();
        try
        {
            var ticket = await _ticketService.BookTicketAsync(requestDto.ShowSeatIds, requestDto.UserId, cancellationToken);
            response.Ticket = ticket;
            response.Status = ResponseStatus.Success;
            return Ok(response);
        }
        catch
        {
            response.Status = ResponseStatus.Failure;
            return BadRequest(response);
        }
    }
}
