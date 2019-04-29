using System.Collections.Generic;
using System.Threading.Tasks;

namespace TruckFinder.Data
{
    public interface IPermitDataService
    {
        Task<IEnumerable<MobileFoodPermit>> GetMobileFoodPermitsAsync(string url);
    }
}
