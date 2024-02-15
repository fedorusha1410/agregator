using Application.Commands.Dtos;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
{
    private readonly IApiAdapter _adapter;
    private readonly ILogger<CancelBookingCommandHandler> _logger;
    public CancelBookingCommandHandler(IApiAdapter apiAdapter, ILogger<CancelBookingCommandHandler> logger)
    {
        _adapter = apiAdapter;
        _logger = logger;
    }
    public Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Cancellation of flight reservation number: " +
                               $"{request.BookingNumber} on Airline: {request.Airline}");
        return _adapter.CancelBook(request.Airline, request.BookingNumber);
    }
}