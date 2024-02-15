using MediatR;

namespace Application.Commands;

public class CancelBookingCommand : IRequest<bool>
{
    /// <summary>
    ///  BookingNumber
    /// </summary>
    /// <example>VB3632</example>
    public string BookingNumber { get; set; }
    
    /// <summary>
    ///  Airline
    /// </summary>
    /// <example>Test Airline</example>
    public string Airline { get; set; }
}