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

        static string ReadString(string prompt, string defValue = null)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return defValue ?? string.Empty;
            }
            return input;
        }

        static decimal ReadDecimal(string prompt, decimal minValue, decimal maxValue, decimal? defValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (string.IsNullOrEmpty(input) && defValue.HasValue)
                {
                    return defValue.Value;
                }
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

        static decimal ReadInteger(string prompt, int minValue, int maxValue, int? defValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (string.IsNullOrEmpty(input) && defValue.HasValue)
                {
                    return defValue.Value;
                }
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

        public static double DistanceBetween(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
        {
            var R = 6371e3; // meters
            var lat1rad = Convert.ToDouble(lat1) * Math.PI / 180;
            var lat2rad = Convert.ToDouble(lat2) * Math.PI / 180;
            var deltalat = (Convert.ToDouble(lat2) - Convert.ToDouble(lat1)) * Math.PI / 180;
            var deltalng = (Convert.ToDouble(lng2) - Convert.ToDouble(lng1)) * Math.PI / 180;

            // using Haversine Formula http://www.movable-type.co.uk/scripts/latlong.html
            var a =
                Math.Sin(deltalat / 2) * Math.Sin(deltalat / 2) +
                Math.Cos(lat1rad) * Math.Cos(lat2rad) *
                Math.Sin(deltalng / 2) * Math.Sin(deltalng / 2);
            var c =
                2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var dist = c * R;

            return dist;
        }

        static async Task TestMain()
        {
            Console.WriteLine("Food Truck Finder");

            // start the data download in background while prompting for input
            var svc = new PermitDataService();
            var task = svc.GetMobileFoodPermitsAsync("https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

            // prompt for location and search filters
            Console.WriteLine();
            const decimal latSomewhereInSF = 37.763104M;
            const decimal lngSomewhereInSF = -122.412188M;
            var lat = ReadDecimal($"Please enter a decimal value for your location's latitude: [{latSomewhereInSF}]", -90, 90, latSomewhereInSF);
            var lng = ReadDecimal($"Please enter a decimal value for your location's longitude: [{lngSomewhereInSF}]", -180, 180, lngSomewhereInSF);
            var facility = ReadInteger("Are you looking for (1) Food Truck, (2) Push Cart, or (3) Either [1]? ", 1, 3, 3);
            var keywords = ReadString("Please enter any additional keywords to search for (i.e. 'tacos'): ").Split(' ');
            Console.WriteLine();

            var permits = await task;

            var active = permits.Where(p => p.Status == "APPROVED" || (p.Status == "REQUESTED" && p.PriorPermit)).ToArray();

            // TODO: lookup lat/lng using geolocation service
            var filtered = active.Where(p => Math.Abs(p.Latitude) > 0 && Math.Abs(p.Longitude) > 0).ToArray();
            
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

            // compute distances
            var sorted =
                from permit in filtered
                let distance = DistanceBetween(lat, lng, permit.Latitude, permit.Longitude)
                orderby distance ascending
                select new {
                    distance = Convert.ToInt32(Math.Round(distance)),
                    permit
                };

            Console.WriteLine($"{sorted.Count()} vendors out of {active.Count()} match your search criteria. Here are the nearest five:");

            var nearest = sorted.Take(5).ToArray();
            for (var ix = 0; ix < nearest.Length; ix++)
            {
                var permit = nearest[ix].permit;
                var distance = nearest[ix].distance;

                Console.WriteLine($"{ix+1}> {permit.FacilityType}: {permit.Applicant} is {distance} meters away");
                Console.WriteLine($"   Address: {permit.Address}");
                Console.WriteLine($"   Items: {permit.FoodItems}");
            }
        }
    }
}
