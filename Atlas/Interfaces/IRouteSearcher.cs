using Atlas.Models;

namespace Atlas.Interfaces;

public interface IRouteSearcher
{
    string Title { get; }
    
    string GeneralLink { get; }

    string GetTripLink(string tripId);
    
    Dictionary<int, string> Cities { get; }
    
    string MessageLink { get; }

    // void Init(DateTime tripDate, int passengers, int fromCity, int toCity);
    
    Task<List<Ride>> SearchRoutesAsync(int fromCityValue, int toCityValue, DateTime date, int passengers, TimeOnly startTime, TimeOnly endTime);
}