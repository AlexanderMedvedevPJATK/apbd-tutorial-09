using Tutorial9.Models;

namespace Tutorial9.DTOs;

public class PaginationTripsDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<Trip> Trips { get; set; } = null!;
}