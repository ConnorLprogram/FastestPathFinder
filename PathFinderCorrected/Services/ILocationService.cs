using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathFinder.Models;


namespace PathFinder.Services
{
    public interface ILocationService
    {
        ValueTask<List<Pathlist>> CompileList(Location startLocation, Location endLocation, int alternateNum);
        IQueryable<Location> GiveLocations();
        IQueryable<Location> SearchLocation(string name);
        ValueTask<Location> AddNewLocationAsync(Location location);
        Location FindLocation(string locationName);
        //object PrintLocationsToConsole();
    }
}
