using System.Text.Json.Serialization;
using Atlas.Interfaces;
using Atlas.Models;
using HtmlAgilityPack;

namespace Atlas.Services;

public class MinskLidaRouteSearcher : IRouteSearcher
{
    private readonly HttpClient _http;
    private readonly Dictionary<string, string> _cityNames = new();
    private List<Ride> _minskLidaTrips = [];

    public MinskLidaRouteSearcher(HttpClient http)
    {
        _http = http;
    }

    public string Title => "MinskLida";
    public string GeneralLink => "https://bilet.minsk-lida.by";

    public string GetTripLink(string tripId) =>
        // _minskLidaTrips.Where(c => c.Id == tripId).Select(c => c.Id).SingleOrDefault() ?? string.Empty;
        GeneralLink;
    
    public Dictionary<int, string> Cities => new()
    {
        { 1, "Минск" },
        { 3, "Лида" },
    };

    public string MessageLink => string.Join("\n",
        _minskLidaTrips.Select(c =>
            $"Новая поездка: {c.Departure.Split(" ")[1]}: [Перейти к маршруту]({GeneralLink})"));

    public async Task<List<Ride>> SearchRoutesAsync(int fromCityValue, int toCityValue, DateTime date, int passengers,
        TimeOnly startTime, TimeOnly endTime)
    {
        var fromCityId = fromCityValue;
        var toCityId = toCityValue;
        var formattedDate = date.ToString("yyyy-MM-dd");
        var url =
            $"https://bilet.minsk-lida.by/schedules?station_from_id=0&station_to_id=0&frame_id=&city_from_id={fromCityId}&places={passengers}&city_to_id={toCityId}&date={formattedDate}";

        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Ошибка HTTP (Minsk-Lida): {response.StatusCode}");
        }

        var content = await response.Content.ReadFromJsonAsync<MinskLidaContent>();

        // Парсинг HTML с помощью HtmlAgilityPack
        var minskLidaTrips = new List<Ride>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(content!.Html);

        // Поиск маршрутов (учитываем пробелы в классе и исключаем маршруты с is-disabled)
        var routeNodes = htmlDoc.DocumentNode.SelectNodes(
            "//div[contains(@class, 'nf-route') and contains(@class, 'js_route') and not(contains(@class, 'is-disabled'))]");
        if (routeNodes == null || !routeNodes.Any())
        {
            Console.WriteLine("Маршруты не найдены.");
            return minskLidaTrips;
        }

        foreach (var routeNode in routeNodes)
        {
            try
            {
                // Извлекаем время отправления и прибытия
                var timeNodes = routeNode.SelectNodes(".//div[contains(@class, 'nf-route-point__time')]");
                if (timeNodes == null || timeNodes.Count < 2) continue;

                var departureTime = timeNodes[0].InnerText.Trim();
                var arrivalTime = timeNodes[1].InnerText.Trim();

                // Извлекаем пункты отправления и назначения
                var locationNodes = routeNode.SelectNodes(".//div[contains(@class, 'nf-route-point__name')]");
                if (locationNodes == null || locationNodes.Count < 2) continue;

                var fromLocation = locationNodes[0].InnerText.Trim();
                var toLocation = locationNodes[1].InnerText.Trim();

                // Извлекаем дату
                var dateNodes = routeNode.SelectNodes(".//div[contains(@class, 'nf-route-point__date')]");
                if (dateNodes == null || dateNodes.Count < 2) continue;

                var tripDate = dateNodes[0].InnerText.Trim();

                // Извлекаем цену
                var priceNode = routeNode.SelectSingleNode(".//div[contains(@class, 'nf-route-order__cost')]//strong");
                if (priceNode == null) continue;
                var priceText = priceNode.InnerText.Trim();
                if (!decimal.TryParse(priceText, out var price)) continue;

                // Извлекаем URL бронирования
                var reservationButton =
                    routeNode.SelectSingleNode(
                        ".//button[contains(@class, 'reservationButton') and contains(@class, 'js_get-bus')]");
                if (reservationButton == null) continue;
                var reservationUrl = reservationButton.GetAttributeValue("data-url", string.Empty);

                // Извлекаем перевозчика
                var carrierNode = routeNode.SelectSingleNode(".//div[contains(@class, 'nf-route__company')]");
                if (carrierNode == null) continue;
                var carrier = carrierNode.InnerText.Trim();

                // Извлекаем расстояние
                var distanceNode =
                    routeNode.SelectSingleNode(
                        ".//div[contains(@class, 'nf-route-description-item') and contains(text(), 'км')]");
                if (distanceNode == null) continue;
                var distanceText = distanceNode.InnerText.Replace("км", "").Trim();
                if (!int.TryParse(distanceText, out var distance)) continue;

                // Извлекаем название маршрута
                var routeNameNode = routeNode.SelectSingleNode(".//div[contains(@class, 'nf-route__search')]");
                if (routeNameNode == null) continue;
                var routeName = routeNameNode.InnerText.Trim();

                // Формируем дату и время отправления/прибытия
                var departureDateTime = DateTime.Parse($"{formattedDate} {departureTime}");
                var arrivalDateTime = DateTime.Parse($"{formattedDate} {arrivalTime}");

                // Проверяем временные рамки
                var startTimeSpan = startTime.ToTimeSpan();
                var endTimeSpan = endTime.ToTimeSpan();
                var departureTimeSpan = departureDateTime.TimeOfDay;

                if (departureTimeSpan >= startTimeSpan && departureTimeSpan <= endTimeSpan)
                {
                    var ride = new Ride
                    {
                        Departure = departureDateTime.ToString("yyyy-MM-dd HH:mm"),
                        Arrival = arrivalDateTime.ToString("yyyy-MM-dd HH:mm"),
                        Price = price,
                        FreeSeats = null, // есть только сколько всего мест в маршрутке
                        Carrier = carrier,
                        Currency = "BYN",
                        RouteName = routeName,
                        ReservationUrl = reservationUrl,
                        From = new Location { Desc = fromLocation },
                        To = new Location { Desc = toLocation },
                        Distance = distance
                    };
                    minskLidaTrips.Add(ride);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга маршрута: {ex.Message}");
                continue;
            }
        }

        _minskLidaTrips = minskLidaTrips;
        return _minskLidaTrips;
    }
}

class MinskLidaContent
{
    [JsonPropertyName("result")] public string Result { get; set; } = string.Empty;

    [JsonPropertyName("html")] public string Html { get; set; } = string.Empty;
}