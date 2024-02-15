using Application.Commands.Dtos;
using Domain;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Application.Commands;

public class BookFlightCommandHandler : IRequestHandler<BookFlightCommand, Booking>
{
    private const string Status = "Created";
    private readonly IApiAdapter _adapter;
    private readonly ILogger<BookFlightCommandHandler> _logger;
    public BookFlightCommandHandler(IApiAdapter apiAdapter, ILogger<BookFlightCommandHandler> logger)
    {
        _adapter = apiAdapter;
        _logger = logger;
    }
    public async Task<Booking?> Handle(BookFlightCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"New Booking flight {request.FlightNumber}");
        var newBooking = new Booking()
        {
            Id = Guid.NewGuid(),
            FlightId = request.FlightNumber,
            PassengerName = request.PassengerName,
            PassengerDocument = request.PassengerDocument,
            ContactNumber = request.ContactNumber,
            BookingNumber = GenerateRandomString(),
            BookingStatus = Status,
            Airline = request.Airline,
        };
        
        var result = await _adapter.BookFlight(newBooking);
        return result ? newBooking : null;
    }

    private static string GenerateRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}