using Application.Commands.Dtos;
using Application.Common;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.Commands;

public class SearchFlightCommandHandler : IRequestHandler<SearchFlightQuery, List<Flight>>
{
    private readonly IApiAdapter _adapter;
    private readonly ILogger<SearchFlightCommandHandler> _logger;
    private readonly IDistributedCache _cache;
    public SearchFlightCommandHandler(IApiAdapter apiAdapter, ILogger<SearchFlightCommandHandler> logger, IDistributedCache cache)
    {
        _adapter = apiAdapter;
        _logger = logger;
        _cache = cache;
    }
    public async Task<List<Flight>?> Handle(SearchFlightQuery request, CancellationToken cancellationToken)
    {

        _logger.LogInformation($"Get a list of all flights: from {request.From}, to: {request.To}, departure time : {request.DepartureTime}: ");
        var key = CacheKeyGenerator.GenerateCacheKey(request.From, request.To, request.DepartureTime);
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);
        List<Flight> resultSet;
        if (cachedData != null)
        {
            resultSet = JsonConvert.DeserializeObject<List<Flight>>(cachedData);
        }
        else
        {
            resultSet = await _adapter.GetFlights(request.From, request.To, request.DepartureTime, key);
            
        }
        if (resultSet == null) return new List<Flight>();
        IEnumerable<Flight> sortedResult = request.SortDirection switch
        {
            SortDirection.Ascending => resultSet.OrderBy(x => x.Price),
            SortDirection.Descending => resultSet.OrderByDescending(x => x.Price),
            _ => resultSet
        };

        sortedResult = ApplyPriceRangeFilter(sortedResult, request.PriceStartFrom, request.PriceEndIn);
        sortedResult = ApplyTransfersFilter(sortedResult, request.Transfers);
        sortedResult = ApplyAirlineFilter(sortedResult, request.Airline);

        return sortedResult.ToList();

    }
    
    IEnumerable<Flight> ApplyPriceRangeFilter(IEnumerable<Flight> flights, decimal? priceStartFrom, decimal? priceEndIn)
    {
        if (priceStartFrom != null)
        {
            flights = flights.Where(x => x.Price >= priceStartFrom);
        }

        if (priceEndIn != null)
        {
            flights = flights.Where(x => x.Price <= priceEndIn);
        }

        return flights;
    }
    
    IEnumerable<Flight> ApplyTransfersFilter(IEnumerable<Flight> flights, int? transfers)
    {
        if (transfers != null)
        {
            flights = flights.Where(x => x.Transfers == transfers);
        }
        
        return flights;
    }
    
    IEnumerable<Flight> ApplyAirlineFilter(IEnumerable<Flight> flights, string? airline)
    {
        if (airline != null)
        {
            flights = flights.Where(x => x.Airline == airline);
        }

        return flights;
    }
}