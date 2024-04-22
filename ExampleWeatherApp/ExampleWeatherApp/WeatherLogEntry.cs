using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleWeatherApp
{
    class WeatherLogEntry
    {
        public DateTime Date { get; set; }
        public double UserEnteredTemperature { get; set; }
        public double APITemperature { get; set; }
        public double AirPressureAtSeaLevel { get; set; }
        public double CloudAreaFraction { get; set; }
        public double WindSpeed { get; set; }
        public double WindFromDirection { get; set; }
        public double Humidity { get; set; }

        public override string ToString()
        {
            return $"Date: {Date}, " +
                   $"User Entered Temperature: {UserEnteredTemperature}, " +
                   $"API Temperature: {APITemperature}, " +
                   $"Air Pressure at Sea Level: {AirPressureAtSeaLevel}, " +
                   $"Cloud Area Fraction: {CloudAreaFraction}, " +
                   $"Wind Speed: {WindSpeed}, " +
                   $"Wind From Direction: {WindFromDirection}, " +
                   $"Humidity: {Humidity}\n";
        }
    }
}
