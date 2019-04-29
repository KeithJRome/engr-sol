using TruckFinder.Data;

namespace TruckFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // on a larger application, these dependencies would likely
            // be wired up via a Dependency Injection container, and not
            // directly as seen here
            ITextConsole io = new ConsoleIO();
            IPermitDataService svc = new PermitDataService();

            var finder = new TruckFinder(io, svc);

            finder.Execute().Wait();
        }
    }
}
