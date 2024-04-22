using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RandomUserAgent;
using System.IO;

namespace ExampleWeatherApp
{
    class WeatherLog
    {
        private readonly HttpClient _client;
        private readonly string _logFilePath = "weatherLog.json";
        public List<WeatherLogEntry> LogEntries;
        public Boolean EntriesFound = false;

        public WeatherLog()
        {
            LoadWeatherLogEntries();
            _client = new HttpClient();
            string userAgent = RandomUa.RandomUserAgent;
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        }

        public async Task<WeatherLogEntry> GetWeatherData(DateTime date, double userEnteredTemperature)
        {
            WeatherLogEntry entry = new WeatherLogEntry();
            entry.Date = date;
            entry.UserEnteredTemperature = userEnteredTemperature;

            // Fetch temperature data from YR API
            WeatherData weatherData = await GetDataFromAPI();
            Timeserie closestTimeSerie = GetClosestTimeSerie(date, weatherData);
            Details details = closestTimeSerie.data.instant.details;

            int decimalPlaces = 2;

            entry.APITemperature = Math.Round(details.air_temperature, decimalPlaces);
            entry.AirPressureAtSeaLevel = Math.Round(details.air_pressure_at_sea_level, decimalPlaces);
            entry.WindSpeed = Math.Round(details.wind_speed, decimalPlaces);
            entry.WindFromDirection = Math.Round(details.wind_from_direction, decimalPlaces);
            entry.Humidity = Math.Round(details.relative_humidity, decimalPlaces);


            return entry;
        }

        public async Task<WeatherData> GetDataFromAPI()
        {

            string latOslo = "59.91";
            string lonOslo = "10.76";

            //Used latitude and longitude for Oslo, Norway
            string path = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latOslo}&lon={lonOslo}";

            HttpResponseMessage responseMessage = await _client.GetAsync(path);

            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("API: Succes!");
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                WeatherData data = JsonConvert.DeserializeObject<WeatherData>(responseBody);
                return data;
            }
            else
            {
                Console.WriteLine("API: Fail!");
                Console.WriteLine(responseMessage.StatusCode);
                throw new HttpRequestException("The HTTP request was not successful.");
            }

        }

        private Timeserie GetClosestTimeSerie(DateTime dateTime, WeatherData weatherData)
        {
            DateTime closestTime = weatherData.properties.timeseries
                    .OrderBy(ts => Math.Abs((ts.time - dateTime).TotalMilliseconds))
                    .FirstOrDefault()?.time ?? DateTime.MinValue;

            if (closestTime != DateTime.MinValue)
            {
                var closestTimeSeries = weatherData.properties.timeseries
                    .FirstOrDefault(ts => ts.time == closestTime);

                if (closestTimeSeries != null)
                {
                    return closestTimeSeries;
                }
                else
                {
                    throw new Exception("Could not find temperature data for the closest timestamp.");
                }
            }
            else
            {
                throw new Exception("Could not find the closest timestamp.");
            }
        }

        public void LoadWeatherLogEntries()
        {
            LogEntries = new List<WeatherLogEntry>();
            if (File.Exists(_logFilePath))
            {
                string existingJson = File.ReadAllText(_logFilePath);
                LogEntries = JsonConvert.DeserializeObject<List<WeatherLogEntry>>(existingJson);
                EntriesFound = true;
            }
        }

        public void SaveWeatherLogEntries()
        {
            string updatedJson = JsonConvert.SerializeObject(LogEntries);
            File.WriteAllText(_logFilePath, updatedJson);
        }

        public void ShowEntries()
        {
            if (EntriesFound || LogEntries.Count > 0)
            {
                foreach (var entry in LogEntries)
                {
                    Console.WriteLine(entry);
                }
            } else
            {
                Console.WriteLine("No weather log entries found");
            }
        }

        public void AddWeatherLogEntry()
        {
            Console.WriteLine("Add Weather Log Entry\n");

            Console.WriteLine("Do you want to use the current time? (Y/N)");
            string useCurrentTime = Console.ReadLine();

            DateTime logTime;

            if (useCurrentTime.ToUpper() == "Y")
            {
                logTime = DateTime.Now;
            }
            else
            {
                Console.WriteLine("Enter custom date and time (YYYY-MM-DD HH:MM:SS):");
                string customDateTimeString = Console.ReadLine();

                if (!DateTime.TryParse(customDateTimeString, out logTime))
                {
                    Console.WriteLine("Invalid date and time format. Using current time.");
                    logTime = DateTime.Now;
                }
            }

            Console.WriteLine("Enter temperature:");

            try
            {
                double temperature = Convert.ToDouble(Console.ReadLine());
                WeatherLogEntry entry = GetWeatherData(logTime, temperature).Result;

                LogEntries.Add(entry);
                Console.WriteLine("Weather log entry added successfully!");
            }
            catch (Exception)
            {
                Console.WriteLine("Wrong value, did not add entry to weather log!");
                return;
            }

            
        }

        public void ViewWeatherReportForDay()
        {
            Console.WriteLine("View Weather Report for Day\n");

            Console.WriteLine("Enter the date for the weather report (YYYY-MM-DD), or press Enter to use today's date:");
            string dateString = Console.ReadLine();

            DateTime date;
            if (string.IsNullOrWhiteSpace(dateString))
            {
                date = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParse(dateString, out date))
                {
                    Console.WriteLine("Invalid date format.");
                    return;
                }
            }

            // Filter log entries for the specified date
            var dayEntries = LogEntries.Where(entry => entry.Date.Date == date.Date).ToList();

            if (dayEntries.Count == 0)
            {
                Console.WriteLine("No weather log entries found for the specified date.");
                return;
            }

            Console.WriteLine($"\nWeather Report for {date.ToShortDateString()}:\n");

            foreach (var entry in dayEntries)
            {
                Console.WriteLine(entry);
            }
        }

        public void ViewAverageWeatherReportForDay()
        {
            Console.WriteLine("View Weather Report for Day\n");

            Console.WriteLine("Enter the date for the weather report (YYYY-MM-DD), or press Enter to use today's date:");
            string dateString = Console.ReadLine();

            DateTime date;
            if (string.IsNullOrWhiteSpace(dateString))
            {
                date = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParse(dateString, out date))
                {
                    Console.WriteLine("Invalid date format.");
                    return;
                }
            }

            // Filter log entries for the specified date
            var dayEntries = LogEntries.Where(entry => entry.Date.Date == date.Date).ToList();

            if (dayEntries.Count == 0)
            {
                Console.WriteLine("No weather log entries found for the specified date.");
                return;
            }

            Console.WriteLine($"\nWeather Report for {date.ToShortDateString()}:\n");

            // Initialize sums for each data field
            double sumUserEnteredTemperature = 0;
            double sumAPITemperature = 0;
            double sumAirPressureAtSeaLevel = 0;
            double sumCloudAreaFraction = 0;
            double sumWindSpeed = 0;
            double sumWindFromDirection = 0;
            double sumHumidity = 0;

            // Calculate sums for each data field
            foreach (var entry in dayEntries)
            {
                sumUserEnteredTemperature += entry.UserEnteredTemperature;
                sumAPITemperature += entry.APITemperature;
                sumAirPressureAtSeaLevel += entry.AirPressureAtSeaLevel;
                sumCloudAreaFraction += entry.CloudAreaFraction;
                sumWindSpeed += entry.WindSpeed;
                sumWindFromDirection += entry.WindFromDirection;
                sumHumidity += entry.Humidity;
            }

            // Calculate averages for each data field
            double avgUserEnteredTemperature = sumUserEnteredTemperature / dayEntries.Count;
            double avgAPITemperature = sumAPITemperature / dayEntries.Count;
            double avgAirPressureAtSeaLevel = sumAirPressureAtSeaLevel / dayEntries.Count;
            double avgCloudAreaFraction = sumCloudAreaFraction / dayEntries.Count;
            double avgWindSpeed = sumWindSpeed / dayEntries.Count;
            double avgWindFromDirection = sumWindFromDirection / dayEntries.Count;
            double avgHumidity = sumHumidity / dayEntries.Count;

            // Display averages
            Console.WriteLine($"Average User Entered Temperature: {avgUserEnteredTemperature:F2} °C");
            Console.WriteLine($"Average API Temperature: {avgAPITemperature:F2} °C");
            Console.WriteLine($"Average Air Pressure at Sea Level: {avgAirPressureAtSeaLevel:F2} hPa");
            Console.WriteLine($"Average Cloud Area Fraction: {avgCloudAreaFraction:F2} %");
            Console.WriteLine($"Average Wind Speed: {avgWindSpeed:F2} m/s");
            Console.WriteLine($"Average Wind From Direction: {avgWindFromDirection:F2} degrees");
            Console.WriteLine($"Average Humidity: {avgHumidity:F2} %");
        }

        public void ViewWeatherReportForWeek()
        {
            Console.WriteLine("Weekly Weather Report\n");

            Console.WriteLine("Enter the date for the weather report (YYYY-MM-DD), or press Enter to use today's date:");
            string startDateString = Console.ReadLine();

            DateTime startDate;
            if (string.IsNullOrWhiteSpace(startDateString))
            {
                startDate = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParse(startDateString, out startDate))
                {
                    Console.WriteLine("Invalid date format.");
                    return;
                }
            }

            DateTime endDate = startDate.AddDays(6);

            List<WeatherLogEntry> weeklyEntries = LogEntries
                .Where(entry => entry.Date >= startDate && entry.Date <= endDate)
                .ToList();

            if (weeklyEntries.Count == 0)
            {
                Console.WriteLine("No weather log entries found for the specified week.");
                return;
            }


            double averageUserTemperature = weeklyEntries.Average(entry => entry.UserEnteredTemperature);
            double averageAPITemperature = weeklyEntries.Average(entry => entry.APITemperature);
            double averageAirPressure = weeklyEntries.Average(entry => entry.AirPressureAtSeaLevel);
            double averageCloudArea = weeklyEntries.Average(entry => entry.CloudAreaFraction);
            double averageWindSpeed = weeklyEntries.Average(entry => entry.WindSpeed);
            double averageWindDirection = weeklyEntries.Average(entry => entry.WindFromDirection);
            double averageHumidity = weeklyEntries.Average(entry => entry.Humidity);


            Console.WriteLine($"Week: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}\n");
            Console.WriteLine($"Average User Entered Temperature: {averageUserTemperature} °C");
            Console.WriteLine($"Average API Temperature: {averageAPITemperature} °C");
            Console.WriteLine($"Average Air Pressure at Sea Level: {averageAirPressure} hPa");
            Console.WriteLine($"Average Cloud Area Fraction: {averageCloudArea} %");
            Console.WriteLine($"Average Wind Speed: {averageWindSpeed} m/s");
            Console.WriteLine($"Average Wind Direction: {averageWindDirection} degrees");
            Console.WriteLine($"Average Humidity: {averageHumidity} %");
        }

        public void ViewWeatherReportForMonth()
        {
            Console.WriteLine("View Weather Report for Month\n");

            Console.WriteLine("Enter the year and month for the weather report (YYYY-MM), or press Enter to use the current year and month:");
            string yearMonthString = Console.ReadLine();

            DateTime yearMonth;
            if (string.IsNullOrWhiteSpace(yearMonthString))
            {
                yearMonth = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParse(yearMonthString, out yearMonth))
                {
                    Console.WriteLine("Invalid year and month format.");
                    return;
                }
            }

            // Get the entries for the specified month
            List<WeatherLogEntry> monthEntries = GetEntriesForMonth(yearMonth.Year, yearMonth.Month);

            // Display the weather report for the month
            if (monthEntries.Count > 0)
            {
                Console.WriteLine($"Weather report for the month of {yearMonth:MMMM yyyy}:");
                double averageUserTemperature = monthEntries.Average(entry => entry.UserEnteredTemperature);
                double averageAPITemperature = monthEntries.Average(entry => entry.APITemperature);
                double averageAirPressure = monthEntries.Average(entry => entry.AirPressureAtSeaLevel);
                double averageCloudArea = monthEntries.Average(entry => entry.CloudAreaFraction);
                double averageWindSpeed = monthEntries.Average(entry => entry.WindSpeed);
                double averageWindDirection = monthEntries.Average(entry => entry.WindFromDirection);
                double averageHumidity = monthEntries.Average(entry => entry.Humidity);

                // Displaying the month report
                Console.WriteLine($"Average User Entered Temperature: {averageUserTemperature} °C");
                Console.WriteLine($"Average API Temperature: {averageAPITemperature} °C");
                Console.WriteLine($"Average Air Pressure at Sea Level: {averageAirPressure} hPa");
                Console.WriteLine($"Average Cloud Area Fraction: {averageCloudArea} %");
                Console.WriteLine($"Average Wind Speed: {averageWindSpeed} m/s");
                Console.WriteLine($"Average Wind Direction: {averageWindDirection} degrees");
                Console.WriteLine($"Average Humidity: {averageHumidity} %");
            }
            else
            {
                Console.WriteLine("No weather log entries found for the specified month.");
            }
        }

        public List<WeatherLogEntry> GetEntriesForMonth(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime endDate = new DateTime(year, month, daysInMonth);

            List<WeatherLogEntry> monthEntries = LogEntries.Where(entry => entry.Date.Year == year && entry.Date.Month == month && entry.Date <= endDate).ToList();
            return monthEntries;
        }
    }
}
