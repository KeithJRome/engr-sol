using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;

namespace TruckFinder.Data
{
    public class PermitDataService : IPermitDataService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<IEnumerable<MobileFoodPermit>> GetMobileFoodPermitsAsync(string url)
        {
            using (var stream = await _client.GetStreamAsync(url).ConfigureAwait(false))
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var csvReader = new CsvReader(reader))
                    {
                        var records = csvReader.GetRecords<MobileFoodPermit>();
                        return records.ToArray(); // cannot defer this enumerable
                    }
                }
            }
        }
    }
}
