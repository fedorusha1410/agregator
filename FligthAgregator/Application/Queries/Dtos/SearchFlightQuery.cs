
using Domain;
using MediatR;

namespace Application.Commands.Dtos;

public class SearchFlightQuery : IRequest<List<Flight>>
{
    /// <summary>
    ///  From 
    /// </summary>
    /// <example>New York</example>
    public string From { get; set; }
    
    /// <summary>
    ///  To 
    /// </summary>
    /// <example>Paris</example>
    public string To { get; set; }
    
    /// <summary>
    ///  DepartureTime 
    /// </summary>
    /// <example>2024-12-02</example>
    public string DepartureTime { get; set; }
    
    /// <summary>
    ///  Airline 
    /// </summary>
    /// <example>Test Airline</example>
    public string? Airline { get; set; } = null;

    /// <summary>
    ///  PriceStartFrom 
    /// </summary>
    /// <example>1000</example>
    public decimal? PriceStartFrom { get; set; } = null;
    
    /// <summary>
    ///  PriceEndIn 
    /// </summary>
    /// <example>1400</example>
    public decimal? PriceEndIn { get; set; } = null;
    
    /// <summary>
    ///  Transfers 
    /// </summary>
    /// <example>1</example>
    public int? Transfers { get; set; } = null;

    /// <summary>
    ///  SortDirection 
    /// </summary>
    /// <example>1 - Descending, 0 - Ascending </example>
    public SortDirection? SortDirection { get; set; } = null;
}