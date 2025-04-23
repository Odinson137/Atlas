using Atlas.Models;

namespace Atlas.Interfaces;

public interface IRouteSearcher
{
    Task<List<Ride>> SearchRoutesAsync(string fromCity, string toCity, DateTime date, int passengers, TimeOnly startTime, TimeOnly endTime);
}