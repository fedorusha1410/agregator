using System.Globalization;
using Application.Commands.Dtos;
using Application.Common;
using Domain;
using Domain.Commands;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;


namespace Application.Tests;

public class SearchFlightsCommandHandlerTests
{
    private Mock<IApiAdapter> mockAdapter;
    private Mock<ILogger<SearchFlightCommandHandler>> mockLogger;

    public SearchFlightsCommandHandlerTests()
    {
        mockAdapter = new Mock<IApiAdapter>();
        mockLogger = new Mock<ILogger<SearchFlightCommandHandler>>();
       
    }

    [Fact]
    public async Task Handle_ReturnsFlightsWhenCacheNotExpired()
        {
            // Arrange
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(opts);
            
            var handler = new SearchFlightCommandHandler(mockAdapter.Object, mockLogger.Object, cache);
            
            var token = new CancellationToken();
            var query = new SearchFlightQuery
            {
                From = "New York",
                To = "Los Angeles",
                DepartureTime = "2024-02-12"
            };
            var key = CacheKeyGenerator.GenerateCacheKey(query.From, query.To, query.DepartureTime);
            var cachedData = "[{\"id\":1,\"airline\":\"DeltaAir\",\"flightNumber\":\"DL123\"," +
                             " \"From\":\"New York\", \"To\": \"Los Angeles\", \"DepartureTime\": \"2024-02-14T12:00:00\"," +
                             " \"ArrivalTime\": \"2024-02-12T11:00:00\", \"Price\": 350.00}]";
            await cache.SetStringAsync(key, cachedData, token);
            await cache.SetStringAsync("test", "result", token);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    
    [Fact]
    public async Task Handle_ReturnsEmptyListWhenNoResults()
    {
        // Arrange
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(opts);
        
        var query = new SearchFlightQuery
        {
            From = "Moscow",
            To = "Minsk",
            DepartureTime = DateTime.Now.AddDays(7).ToString(CultureInfo.InvariantCulture),
            SortDirection = SortDirection.Descending,
            PriceStartFrom = 100,
            PriceEndIn = 500,
            Transfers = 1,
            Airline = "Test"
        };
        var handler = new SearchFlightCommandHandler(mockAdapter.Object,mockLogger.Object, cache);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Count);
    }
    
    [Fact]
    public async Task Handle_ReturnsSortedFlightList()
    {
        // Arrange
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(opts);
        var query = new SearchFlightQuery
        {
            From = "New York",
            To = "Los Angeles",
            DepartureTime = "2024-02-12",
            SortDirection = SortDirection.Descending,
            Airline = "DeltaAir"
        };
        var key = CacheKeyGenerator.GenerateCacheKey(query.From, query.To, query.DepartureTime);
        var flights = new List<Flight>()
        {
            new()
            {
                Airline = "DeltaAir",
                DepartureTime = Convert.ToDateTime("2024-02-12T13:02:00"),
                From = "New York", To = "Los Angeles",
                ArrivalTime = Convert.ToDateTime("2024-02-12T11:00:00"), FlightNumber = "DL123",
                Price = 1000,
                Id = 103,
                Transfers = 1,
            },
            new()
            {
                Airline = "DeltaAir",
                DepartureTime = Convert.ToDateTime("2024-02-12T03:00:00"),
                From = "New York", To = "Los Angeles",
                ArrivalTime = Convert.ToDateTime("2024-02-12T11:00:00"),
                FlightNumber = "DL123",
                Price = 218,
                Id = 104,
                Transfers = 3,
            },
            new()
            {
                Airline = "DeltaAir",
                DepartureTime = Convert.ToDateTime("2024-02-12T10:30:00"),
                From = "New York", To = "Los Angeles",
                ArrivalTime = Convert.ToDateTime("2024-02-12T13:45:00"),
                FlightNumber = "AA456",
                Price = 305,
                Id = 105,
                Transfers = 0,
            }
        };
        var jsonData = JsonConvert.SerializeObject(flights);
        await cache.SetStringAsync(key, jsonData);

        var handler = new SearchFlightCommandHandler(mockAdapter.Object, mockLogger.Object, cache);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        var expectedValue = flights.OrderByDescending(x => x.Price).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.True(result.First().Price == expectedValue.First().Price);
    }
}