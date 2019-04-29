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

        static async Task TestMain()
        {
            var svc = new PermitDataService();
            var permits = await svc.GetMobileFoodPermitsAsync("https://data.sfgov.org/api/views/rqzj-sfat/rows.csv");

            Console.WriteLine($"Permits: {permits.Count()}");
        }
    }
}
