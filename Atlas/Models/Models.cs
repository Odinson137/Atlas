namespace Atlas.Models;

public class SearchResponse
{
    public List<CalendarEntry> Calendar { get; set; } = new();
    public List<Ride> Rides { get; set; } = new();
}

public class CalendarEntry
{
    public string Date { get; set; } = string.Empty;
    public List<MinPrice> MinPrices { get; set; } = new();
    public int RideCount { get; set; }
}

public class MinPrice
{
    public string Currency { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal MaxPrice { get; set; }
}

public class Ride
{
    public string Arrival { get; set; } = string.Empty;
    public AtlasMeta? AtlasMeta { get; set; }
    public List<string> Benefits { get; set; } = new();
    public List<string> BookFields { get; set; } = new();
    public Bus? Bus { get; set; }
    public string Carrier { get; set; } = string.Empty;
    public string CarrierID { get; set; } = string.Empty;
    public string Connector { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Departure { get; set; } = string.Empty;
    public List<Stop> DischargeStops { get; set; } = new();
    public bool DynamicMode { get; set; }
    public int FlightPopular { get; set; }
    public int? FreeSeats { get; set; }
    public List<int> FreeSeatsCount { get; set; } = new();
    public Freighter Freighter { get; set; } = new();
    public Location From { get; set; } = new();
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool OnlineRefund { get; set; }
    public string Partner { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public List<string> PaymentTypes { get; set; } = new();
    public List<Stop> PickupStops { get; set; } = new();
    public decimal Price { get; set; }
    public string? SaasId { get; set; }
    public List<string> SaleTypes { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public Location To { get; set; } = new();
    public int ValidBefore { get; set; }
    public ServiceInfo Animals { get; set; } = new();
    public ServiceInfo Luggage { get; set; } = new();
    public Dictionary<string, object> CarpoolMeta { get; set; } = new();
    public List<string> CarrierPhones { get; set; } = new();
    public Dictionary<string, object> Driver { get; set; } = new();
    public DynamicConfig DynamicConfig { get; set; } = new();
    public decimal Fee { get; set; }
    public LegalInfo Legal { get; set; } = new();
    public decimal OnlinePrice { get; set; }
    public string RefundConditions { get; set; } = string.Empty;
    public string RideType { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public int TicketLimit { get; set; }
    public FrameInfo Frame { get; set; } = new();
    public int Distance { get; set; }
    public Coordinates DepartureCoords { get; set; } = new();
    public Coordinates ArrivalCoords { get; set; } = new();
    public string ExtraDescription { get; set; } = string.Empty;
    public string ReservationUrl { get; set; } = string.Empty;
}

public class AtlasMeta
{
    public MilesInfo Miles { get; set; } = new();
}

public class MilesInfo
{
    public MilesItem Item { get; set; } = new();
    public string State { get; set; } = string.Empty;
    public decimal Cash { get; set; }
    public decimal Card { get; set; }
}

public class MilesItem
{
    public string Currency { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PriceOnline { get; set; }
    public decimal Rate { get; set; }
    public decimal RateCard { get; set; }
}

public class Bus
{
    public string Mark { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Reg { get; set; } = string.Empty;
    public string Branding { get; set; } = string.Empty;
    public Color? Color { get; set; }
}

public class Color
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class Stop
{
    public string Id { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public bool Important { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public bool Dynamic { get; set; }
    public bool Seating { get; set; }
    public string Datetime { get; set; } = string.Empty;
}

public class Freighter
{
    public int Id { get; set; }
    public string Authority { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public string Unp { get; set; } = string.Empty;
    public string WorkingTime { get; set; } = string.Empty;
}

public class Location
{
    public string Id { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}

public class ServiceInfo
{
    public bool Available { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class DynamicConfig
{
    public int PrepareTime { get; set; }
}

public class LegalInfo
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Tin { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class FrameInfo
{
    public bool Use { get; set; }
}

public class Coordinates
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}