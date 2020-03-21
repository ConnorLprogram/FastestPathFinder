using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathFinder.Models;


namespace PathFinder.Services
{
    public interface ILocationService
    {

        IQueryable<Location> GiveLocations();
        IQueryable<Location> SearchLocation(string name);
        ValueTask<Location> AddNewLocationAsync(Location location);
        Location FindLocation(string locationName);
        Location FindLocationByDistance(double distance);
        List<Location> FindCloseLocations(List<Location> allLocations);
        Pathlist FindClosest(List<Location> allLocations, double endLatitude, double endLongitude);
        ValueTask<List<Pathlist>> CompileList(Location startLocation, Location endLocation);

        //object PrintLocationsToConsole();
    }
}
