using System.Text.Json;
using Atlas.Interfaces;
using Atlas.Models;

namespace Atlas.Services;

public class AtlasRouteSearcher : IRouteSearcher
{
    private readonly HttpClient _http;
    private readonly Dictionary<string, string> _cityNames;
    private readonly string _fromCityName;
    private string _toCityName;
    private string _formattedDate;
    private int _passengers;
    private string _fromCity;
    private string _toCity;
    private List<Ride> _atlasTrips;

    public AtlasRouteSearcher(HttpClient http, Dictionary<string, string> cityNames, string fromCityName,
        string toCityName, string formattedDate, int passengers, string fromCity, string toCity)
    {
        _http = http;
        _cityNames = cityNames;
        _fromCityName = fromCityName;
        _toCityName = toCityName;
        _formattedDate = formattedDate;
        _passengers = passengers;
        _fromCity = fromCity;
        _toCity = toCity;
    }

    public string Title => "Atlas";

    public string GeneralLink =>
        $"https://atlasbus.by/Маршруты/{_fromCityName}/{_toCityName}?date={_formattedDate}&passengers={_passengers}&from={_fromCity}&to={_toCity}";

    public string GetTripLink(string tripId) =>
        $"https://atlasbus.by/booking/{tripId}?passengers=1&from={_fromCity}&to={_toCity}&date={_formattedDate}&pickup=&discharge=";

    public string LinkTitle => "Открыть на Atlas";

    public string MessageLink => string.Join("\n", _atlasTrips.Select(c => $"Новая поездка: {c.Departure.Split(" ")[1]}: [Перейти к маршруту]({GetTripLink(c.Id)})"));

    public async Task<List<Ride>> SearchRoutesAsync(string fromCity, string toCity, DateTime date, int passengers,
        TimeOnly startTime, TimeOnly endTime)
    {
        var formattedDate = date.ToString("yyyy-MM-dd");
        var url =
            $"https://atlasbus.by/api/search?from_id={fromCity}&to_id={toCity}&calendar_width=30&date={formattedDate}&passengers={passengers}";
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
            if (ride.FreeSeats > 0)
            {
                var departure = DateTime.Parse(ride.Departure);
                var arrival = DateTime.Parse(ride.Arrival);

                var startTimeSpan = startTime.ToTimeSpan();
                var endTimeSpan = endTime.ToTimeSpan();
                var departureTime = departure.TimeOfDay;

                if (departureTime >= startTimeSpan && departureTime <= endTimeSpan)
                {
                    ride.Departure = departure.ToString("yyyy-MM-dd HH:mm");
                    ride.Arrival = arrival.ToString("yyyy-MM-dd HH:mm");
                    ride.From = new Location { Desc = _cityNames[fromCity] };
                    ride.To = new Location { Desc = _cityNames[toCity] };
                    _atlasTrips.Add(ride);
                }
            }
        }

        return _atlasTrips;
    }
}