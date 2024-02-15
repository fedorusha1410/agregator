
using Domain;
using MediatR;

namespace Application.Commands.Dtos;

public class BookFlightCommand : IRequest<Booking>
{
    /// <summary>
    ///  FlightNumber
    /// </summary>
    /// <example>2456</example>
    public int FlightNumber { get; set; }
    
    /// <summary>
    ///  Airline
    /// </summary>
    /// <example>Test Airline</example>
    public string Airline { get; set; }
    
    /// <summary>
    ///  PassengerName
    /// </summary>
    /// <example>Test</example>
    public string PassengerName { get; set; }
    
    /// <summary>
    ///  PassengerDocument
    /// </summary>
    /// <example>BV492857</example>
    public string PassengerDocument { get; set; }
    
    /// <summary>
    ///  PassengerDocument
    /// </summary>
    /// <example>12446684534</example>
    public string ContactNumber { get; set; }
}