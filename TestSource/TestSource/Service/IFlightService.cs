using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TestSource.Model;

namespace TestSource.Service;

public interface IFlightService
{
    Task<List<Flight>> GetFlights(string source, string from, string to, string depTime);
    Task<bool> BookFlight(string source, object? body);
    Task CancelBook(string source, string bookingNumber);
    Task<Booking?> CheckBook(string source, string bookingNumber);
}