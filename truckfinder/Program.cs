using System;
using System.Linq;
using System.Threading.Tasks;
using TruckFinder.Data;

namespace TruckFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMain().Wait();
        }

        static string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static decimal ReadDecimal(string prompt, decimal minValue, decimal maxValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (decimal.TryParse(input, out decimal output))
                {
                    if (output >= minValue && output <= maxValue)
                    {
                        return output;
                    }
                }
                Console.WriteLine($"'{input}' is not a valid value. Please try again.");
            }
        }

        static decimal ReadInteger(string prompt, int minValue, int maxValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (int.TryParse(input, out int output))
                {
                    if (output >= minValue && output <= maxValue)
                    {
                        return output;
                    }
                }
                Console.WriteLine($"'{input}' is not a valid value. Please try again.");
            }
        }

        static async Task TestMain()
        {
            Console.WriteLine("Food Truck Finder");

            // start the data download in background while prompting for input
            var svc = new PermitDataService();
            var task = svc.GetMobileFoodPermitsAsync("https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

            // prompt for location and search filters
            Console.WriteLine();
            var lat = ReadDecimal("Please enter a decimal value for your location's latitude: ", -90, 90);
            var lng = ReadDecimal("Please enter a decimal value for your location's longitude: ", 0, 360);
            var facility = ReadInteger("Are you looking for (1) Food Truck, (2) Push Cart, or (3) Either? ", 1, 3);
            var keywords = ReadString("Please enter any additional keywords to search for (i.e. 'tacos'): ").Split(' ');
            Console.WriteLine();

            var permits = await task;
            Console.WriteLine($"Total Mobile Permits Found: {permits.Count()}");

            var filtered = permits.Where(p => p.Status == "APPROVED" || (p.Status == "REQUESTED" && p.PriorPermit)).ToArray();
            Console.WriteLine($"Active Permits: {filtered.Count()}");

            // filters for trucks-only and carts-only options
            switch (facility)
            {
                case 1:
                    filtered = filtered.Where(p => p.FacilityType == "Truck").ToArray();
                    break;
                case 2:
                    filtered = filtered.Where(p => p.FacilityType == "Push Cart").ToArray();
                    break;
            }

            // filter based on additional keywords, if supplied
            if (keywords.Any())
            {
                filtered = filtered.Where(
                    p => p.MatchesAnyKeywords(keywords)).ToArray();
            }

            Console.WriteLine($"Net: {filtered.Count()}");
        }
    }
}
