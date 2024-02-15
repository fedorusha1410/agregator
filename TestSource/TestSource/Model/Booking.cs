namespace TestSource.Model;

public class Booking
{
    public Guid Id { get; set; }
    public int FlightId { get; set; }
    public string PassengerName { get; set; }
    public string PassengerDocument { get; set; }
    public string ContactNumber { get; set; }
    public string BookingNumber { get; set; }
    public string BookingStatus { get; set; }
}