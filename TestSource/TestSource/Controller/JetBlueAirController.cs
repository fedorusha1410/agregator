﻿using Microsoft.AspNetCore.Mvc;
using TestSource.Model;
using TestSource.Service;

namespace TestSource;

[Route("api/jetBlueAir")]
public class JetBlueAirController : BaseController
{
    private const string Source = "jetBlueAir.json";
    private const string BookSource = "jetBlueBook.json";
    public JetBlueAirController(IFlightService flightService) : base(flightService)
    {
    }
    
    [Route("from/{from}/to/{to}/departureTime/{departureTime}")]
    [HttpGet]
    public Task<List<Flight>> Get(string from, string to, string departureTime)
    {
        return FlightService.GetFlights(Source, from,to, departureTime);
    }
    
    [HttpPost]
    public Task<bool> Book([FromBody] object body)
    {
        return FlightService.BookFlight(BookSource, body);
    }

    [Route("book")]
    [HttpDelete]
    public Task Cancel([FromQuery] string number)
    {
        return FlightService.CancelBook(BookSource, number);
    }

    
}