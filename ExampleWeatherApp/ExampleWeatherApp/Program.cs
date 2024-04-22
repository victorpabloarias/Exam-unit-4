using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleWeatherApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WeatherLog log = new WeatherLog();
            bool isProgramActive = true;

            while (isProgramActive)
            {
                Console.Clear();
                Console.WriteLine("===========================================");
                Console.WriteLine("|              Weather App Menu           |");
                if (log.EntriesFound)
                {
                    Console.WriteLine($"  Found {log.LogEntries.Count} Entries in Weather Log");
                }
                Console.WriteLine("===========================================");
                Console.WriteLine("| 1. Add Weather Log Entry               |");
                Console.WriteLine("| 2. View Weather Log Entries            |");
                Console.WriteLine("| 3. View Weather Report for Day         |");
                Console.WriteLine("| 4. View Weather Report for Week        |");
                Console.WriteLine("| 5. View Weather Report for Month       |");
                Console.WriteLine("| 6. Exit                                |");
                Console.WriteLine("===========================================");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine("\n\n\n");
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        log.AddWeatherLogEntry();
                        Console.ReadKey();
                        break;
                    case "2":
                        log.ShowEntries();
                        Console.WriteLine("\n\nPress any key to continue: ");
                        Console.ReadKey();
                        break;
                    case "3":
                        log.ViewWeatherReportForDay();
                        //log.ViewAverageWeatherReportForDay();
                        Console.ReadKey();
                        break;
                    case "4":
                        log.ViewWeatherReportForWeek();
                        Console.ReadKey();
                        break;
                    case "5":
                        log.ViewWeatherReportForMonth();
                        Console.ReadKey();
                        break;
                    case "6":
                        isProgramActive = false;
                        log.SaveWeatherLogEntries();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, Please try again.");
                        break;
                }
            }
        }
    }
}
