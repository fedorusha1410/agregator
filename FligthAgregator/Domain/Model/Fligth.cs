namespace Domain;

public class Flight
{
    public int Id { get; set; }
    public string Airline { get; set; }
    public string FlightNumber { get; set; }
    public int Transfers { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
}