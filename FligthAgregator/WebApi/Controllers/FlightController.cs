using Application.Commands;
using Application.Commands.Dtos;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[Route("api/[controller]")]

public class FlightController : Controller
{
    private ISender _mediator = null!;

    private ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    
    [HttpGet]
    public async Task<List<Flight>> SearchFlight([FromQuery] SearchFlightQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<Booking> BookFlight([FromBody] BookFlightCommand bookFlightCommand,
        CancellationToken cancellationToken)
    {
        return await Mediator.Send(bookFlightCommand, cancellationToken);
    }
    [Route("airlines/{airline}/book/bookingNumber/{number}")]
    [HttpDelete]
    public async Task<ActionResult> CancelBook(string number, string airline, CancellationToken cancellationToken)
    {
         await Mediator .Send(new CancelBookingCommand(){ BookingNumber = number, Airline = airline}, cancellationToken);
         return NoContent();
    }
   
}