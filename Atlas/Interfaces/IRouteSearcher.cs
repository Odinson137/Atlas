using Atlas.Models;

namespace Atlas.Interfaces;

public interface IRouteSearcher
{
    string Title { get; }
    
    string GeneralLink { get; }

    string GetTripLink(string tripId);
    
    string LinkTitle { get; }
    
    string MessageLink { get; }
    
    Task<List<Ride>> SearchRoutesAsync(string fromCity, string toCity, DateTime date, int passengers, TimeOnly startTime, TimeOnly endTime);
}