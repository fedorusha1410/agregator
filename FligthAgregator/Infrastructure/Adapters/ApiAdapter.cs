using System.Net.Http.Json;
using Domain;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Adapters;

public class ApiAdapter : IApiAdapter
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ApiAdapter> _logger;
    private HttpClient _client;
    private const string DeltaBaseUrl = "http://localhost:6001/api/deltaAir";
    private const string JetBlueBaseUrl = "http://localhost:6001/api/jetBlueAir";
    
    public ApiAdapter(IHttpClientFactory httpClient, IDistributedCache cache, ILogger<ApiAdapter> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }
    public async Task<List<Flight>?> GetFlights(string from, string to, string departureTime, string key)
    {
        _client = _httpClient.CreateClient();
        var deltaFlights = await getFlightsFromApi(DeltaBaseUrl, from, to, departureTime);
        var jetBlueFlights = await getFlightsFromApi(JetBlueBaseUrl, from, to, departureTime);

        if (jetBlueFlights != null) deltaFlights?.AddRange(jetBlueFlights);
        var duration = TimeSpan.FromHours(2);
        var jsonData = JsonConvert.SerializeObject(deltaFlights);
        await _cache.SetStringAsync(key, jsonData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = duration,
        });
        return deltaFlights;

    }
    
    public async Task<bool> CancelBook(string airline, string bookNumber)
    {
        try
        {
            _client = _httpClient.CreateClient();
            var queryParams = new System.Collections.Specialized.NameValueCollection
            {
                { "number", bookNumber }
            };
            var queryString = string.Join("?", queryParams.AllKeys
                .Select(key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"));
            string? fullUrl;
            switch (airline)
            {
                case "DeltaAir":
                    _client.BaseAddress = new Uri(DeltaBaseUrl);
                    fullUrl = DeltaBaseUrl +"/book"+ "?" + queryString;
                    break;
                case "JetBlueAir":
                    _client.BaseAddress = new Uri(JetBlueBaseUrl);
                    fullUrl = JetBlueBaseUrl +"/book"+ "?" + queryString;
                    break;
                default:
                    fullUrl = string.Empty;
                    break;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to cancel book in sources. Error message: {e.Message}");
            return false;
        }
      
    }

    public async Task<bool> BookFlight(Booking newBooking)
    {

        var requestBody = JsonConvert.SerializeObject(newBooking);

        switch (newBooking.Airline)
        {
            case "DeltaAir":
                return await postBookingAsync(DeltaBaseUrl, newBooking);
            case "JetBlueAir":
                return await postBookingAsync(JetBlueBaseUrl, newBooking);
            default:
                return false;
        }
    }
    
    private async Task<List<Flight>?> getFlightsFromApi(string baseUrl, string from, string to, string departureTime)
    {
        try
        {
            _client = _httpClient.CreateClient();
            _client.BaseAddress = new Uri(baseUrl);
            _client.Timeout = TimeSpan.FromSeconds(90);

            var queryParams = new System.Collections.Specialized.NameValueCollection
            {
                { "from", from },
                { "to", to },
                { "departureTime", departureTime }
            };
            var queryString = string.Join("/", queryParams.AllKeys
                .Select(key => $"{Uri.EscapeDataString(key)}/{Uri.EscapeDataString(queryParams[key])}"));

            var fullUrl = baseUrl + "/" + queryString;
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Flight>>(responseBody);
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError($"Timeout occurred while retrieving flights: {e.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve data from sources. Error message: {ex.Message}");
            return null;
        }
       
    }
    private async Task<bool> postBookingAsync(string baseUrl, Booking content)
    {
        try
        {
            _client = _httpClient.CreateClient();
            _client.BaseAddress = new Uri(baseUrl);

            var response = await _client.PostAsJsonAsync(baseUrl, content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseBody = await response.Content.ReadAsStringAsync();
            return bool.Parse(responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save booking into sources. Error message: {ex.Message}");
            return false;
        }
      
    }
}