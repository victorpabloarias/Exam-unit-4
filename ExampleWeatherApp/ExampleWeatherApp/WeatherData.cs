
using System;

public class WeatherData
{
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }

    public static implicit operator double(WeatherData v)
    {
        throw new NotImplementedException();
    }
}

public class Geometry
{
    public string type { get; set; }
    public float[] coordinates { get; set; }
}

public class Properties
{
    public Meta meta { get; set; }
    public Timeserie[] timeseries { get; set; }
}

public class Meta
{
    public DateTime updated_at { get; set; }
    public Units units { get; set; }
}

public class Units
{
    public string air_pressure_at_sea_level { get; set; }
    public string air_temperature { get; set; }
    public string cloud_area_fraction { get; set; }
    public string precipitation_amount { get; set; }
    public string relative_humidity { get; set; }
    public string wind_from_direction { get; set; }
    public string wind_speed { get; set; }
}

public class Timeserie
{
    public DateTime time { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public Instant instant { get; set; }
}

public class Instant
{
    public Details details { get; set; }
}

public class Details
{
    public float air_pressure_at_sea_level { get; set; }
    public float air_temperature { get; set; }
    public float cloud_area_fraction { get; set; }
    public float relative_humidity { get; set; }
    public float wind_from_direction { get; set; }
    public float wind_speed { get; set; }
}