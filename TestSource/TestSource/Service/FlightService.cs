using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestSource.Model;

namespace TestSource.Service;

public class FlightService : IFlightService
{
    private const string BookingNumberParam = "BookingNumber";
    private const string Format = "yyyy-MM-dd";

   public async Task<List<Flight>> GetFlights(string source, string from, string to, string depTime)
    {
        var json = await File.ReadAllTextAsync(source);

        var flights = JsonConvert.DeserializeObject<List<Flight>>(json);
        var dateTime = DateTime.ParseExact(depTime, Format, CultureInfo.InvariantCulture);

       
        var filteredObjects = flights!
            .Where(obj => obj.From == from && obj.To == to && obj.DepartureTime.Date == dateTime.Date ).ToList();
        return filteredObjects;
    }

    public async Task<bool> BookFlight(string source, object? body)
    {
        if (body == null)
        {
            return false;
        }
        var bookings = (File.Exists(source) ? JsonConvert.DeserializeObject<List<Booking>>(await File.ReadAllTextAsync(source)) : new List<Booking>()) ??
                       new List<Booking>();
        bookings.Add(JsonConvert.DeserializeObject<Booking>(body.ToString()));
        var updatedJson = JsonConvert.SerializeObject(bookings, Formatting.Indented);
        await File.WriteAllTextAsync(source, updatedJson);
        return true;
    }

    public async Task CancelBook(string source, string bookingNumber)
    {
        var json = await File.ReadAllTextAsync(source);
        var jsonArray = JArray.Parse(json);
        
        var obj = jsonArray.FirstOrDefault(obj => obj[BookingNumberParam]?.ToString() == bookingNumber);
        
        if (obj != null) jsonArray.Remove(obj);
        
        var updatedJson = jsonArray.ToString();
        await File.WriteAllTextAsync(source, updatedJson);
    }
    public async Task<Booking?> CheckBook(string source, string bookingNumber)
    {
        var json = await File.ReadAllTextAsync(source);
        var bookings = JsonConvert.DeserializeObject<List<Booking>>(json);
        
        var result = bookings?.FirstOrDefault(obj => obj.BookingNumber == bookingNumber);
        return result;

    }
    
}