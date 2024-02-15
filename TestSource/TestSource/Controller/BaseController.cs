using Microsoft.AspNetCore.Mvc;
using TestSource.Model;
using TestSource.Service;

namespace TestSource;

public abstract class BaseController : Controller
{
    protected  readonly IFlightService FlightService;

    protected BaseController(IFlightService flightService)
    {
        FlightService = flightService;
    }


}