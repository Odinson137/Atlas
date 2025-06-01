using System.Text.Json;
using Atlas.Interfaces;
using Atlas.Models;

namespace Atlas.Services;

public class AtlasRouteSearcher : IRouteSearcher
{
    private readonly HttpClient _http;
    private string _fromCityName;
    private string _toCityName;
    private string _formattedDate;
    private int _passengers;
    private int _fromCityValue;
    private int _toCityValue;
    private List<Ride> _atlasTrips;

    public AtlasRouteSearcher(HttpClient http)
    {
        _http = http;
    }

    // public void Init(DateTime tripDate, int passengers, int fromCity, int toCity)
    // {
    //     _fromCityName = Cities[fromCity];
    //     _toCityName = Cities[toCity];
    //     _formattedDate = tripDate.ToString("yyyy-MM-dd");
    //     _passengers = passengers;
    //     _fromCityValue = fromCity;
    //     _toCityValue = toCity;
    // }

    
    public string Title => "Atlas";

    public string GeneralLink =>
        $"https://atlasbus.by/Маршруты/{_fromCityName}/{_toCityName}?date={_formattedDate}&passengers={_passengers}&from={_fromCityValue}&to={_toCityValue}";

    public string GetTripLink(string tripId) =>
        $"https://atlasbus.by/booking/{tripId}?passengers=1&from={_fromCityValue}&to={_toCityValue}&date={_formattedDate}&pickup=&discharge=";

    public Dictionary<int, string> Cities => new()
    {
        { 1, "Минск" },
        { 2, "Берёзовка" },
        { 3, "Лида" }
    };

    private readonly Dictionary<int, string> _cityToAtlasMapping = new()
    {
        { 1, "c625144" }, // Минск
        { 2, "c630166" }, // Берёзовка
        { 3, "c626081" }, // Лида
    };
    
    public string MessageLink => string.Join("\n",
        _atlasTrips.Select(c =>
            $"Новая поездка: {c.Departure.Split(" ")[1]}: [Перейти к маршруту]({GetTripLink(c.Id)})"));

    public async Task<List<Ride>> SearchRoutesAsync(int fromCityValue, int toCityValue, DateTime date, int passengers,
        TimeOnly startTime, TimeOnly endTime)
    {
        var formattedDate = date.ToString("yyyy-MM-dd");
        var url =
            $"https://atlasbus.by/api/search?from_id={_cityToAtlasMapping[fromCityValue]}&to_id={_cityToAtlasMapping[toCityValue]}&calendar_width=30&date={formattedDate}&passengers={passengers}";
        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Ошибка HTTP (Atlas): {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var searchResponse = JsonSerializer.Deserialize<SearchResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (searchResponse == null || searchResponse.Rides == null)
        {
            throw new Exception("Не удалось распарсить ответ от API Atlas.");
        }

        _atlasTrips = [];
        foreach (var ride in searchResponse.Rides)
        {
            if (!(ride.FreeSeats > 0)) continue;
            
            var departure = DateTime.Parse(ride.Departure);
            var arrival = DateTime.Parse(ride.Arrival);

            var startTimeSpan = startTime.ToTimeSpan();
            var endTimeSpan = endTime.ToTimeSpan();
            var departureTime = departure.TimeOfDay;

            if (departureTime < startTimeSpan || departureTime > endTimeSpan) continue;
            
            ride.Departure = departure.ToString("yyyy-MM-dd HH:mm");
            ride.Arrival = arrival.ToString("yyyy-MM-dd HH:mm");
            // ride.From = new Location { Desc = _cityNames[fromCityValue] };
            // ride.To = new Location { Desc = _cityNames[toCityValue] };
            _atlasTrips.Add(ride);
        }

        _fromCityName = Cities[fromCityValue];
        _toCityName = Cities[toCityValue];
        _formattedDate = date.ToString("yyyy-MM-dd");
        _passengers = passengers;
        _fromCityValue = fromCityValue;
        _toCityValue = toCityValue;
        
        return _atlasTrips;
    }
}