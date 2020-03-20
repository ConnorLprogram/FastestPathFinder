using System.Linq;
using System.Threading.Tasks;
using PathFinder.Models;

namespace PathFinder.Brokers
{
    public interface ILocationStorage
    {
        ValueTask<Location> InsertLocationAsync(Location location);

        ValueTask<Location> FindLocationAsync(string name);
        
        ValueTask<Location> FindLocationByDistanceAsync(double distance);

        IQueryable<Location> SelectAllLocationsAsync();

        IQueryable<Pathlist> SelectAllClosestAsync();
    }
}
