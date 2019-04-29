using System;
using System.Linq;
using System.Threading.Tasks;
using TruckFinder.Data;

namespace TruckFinder
{
    public class TruckFinder
    {
        private readonly ITextConsole io;
        private readonly IPermitDataService svc;

        public TruckFinder(ITextConsole io, IPermitDataService svc)
        {
            this.io = io;
            this.svc = svc;
        }

        public async Task Execute()
        {
            io.WriteLine("Food Truck Finder");

            // start the data download in background while prompting for input
            var task = svc.GetMobileFoodPermitsAsync("https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

            // prompt for location and search filters
            io.WriteLine();
            const decimal latSomewhereInSF = 37.763104M;
            const decimal lngSomewhereInSF = -122.412188M;
            var lat = io.ReadDecimal($"Please enter a decimal value for your location's latitude: [{latSomewhereInSF}]", -90, 90, latSomewhereInSF);
            var lng = io.ReadDecimal($"Please enter a decimal value for your location's longitude: [{lngSomewhereInSF}]", -180, 180, lngSomewhereInSF);
            var facility = io.ReadInteger("Are you looking for (1) Food Truck, (2) Push Cart, or (3) Either [1]? ", 1, 3, 3);
            var keywords = io.ReadString("Please enter any additional keywords to search for (i.e. 'tacos'): ").Split(' ');
            io.WriteLine();

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
                    p => MatchesAnyKeywords(p, keywords)).ToArray();
            }

            // compute distances
            var sorted =
                from permit in filtered
                let distance = DistanceBetween(lat, lng, permit.Latitude, permit.Longitude)
                orderby distance ascending
                select new
                {
                    distance = Convert.ToInt32(Math.Round(distance)),
                    permit
                };

            io.WriteLine($"{sorted.Count()} vendors out of {active.Count()} match your search criteria. Here are the nearest five:");

            var nearest = sorted.Take(5).ToArray();
            for (var ix = 0; ix < nearest.Length; ix++)
            {
                var permit = nearest[ix].permit;
                var distance = nearest[ix].distance;

                io.WriteLine($"{ix + 1}> {permit.FacilityType}: {permit.Applicant} is {distance} meters away");
                io.WriteLine($"   Address: {permit.Address}");
                io.WriteLine($"   Items: {permit.FoodItems}");
            }
        }

        public static bool MatchesAnyKeywords(MobileFoodPermit permit, string[] keywords)
        {
            var applicant = permit.Applicant ?? string.Empty;
            var address = permit.Address ?? string.Empty;
            var fooditems = permit.FoodItems ?? string.Empty;
            foreach (var keyword in keywords)
            {
                if (applicant.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                if (address.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                if (fooditems.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
    }
}
