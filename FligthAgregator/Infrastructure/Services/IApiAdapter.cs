using Domain;

namespace Infrastructure.Services;

public interface IApiAdapter
{
    Task<List<Flight>?> GetFlights(string from, string to, string departureTime, string key);
    Task<bool> CancelBook(string airline, string bookNumber);
    Task<bool> BookFlight(Booking newBooking);
}