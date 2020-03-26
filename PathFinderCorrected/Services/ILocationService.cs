using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathFinder.Models;


namespace PathFinder.Services
{
    public interface ILocationService
    {
        ValueTask<List<Pathlist>> CompileList(Location startLocation, Location endLocation);
        List<Location> FindCloseLocations(List<Location> allLocations);
        Pathlist FindClosest(List<Location> allLocations, Location endLocation);
        ValueTask<List<Pathlist>> CompileAlternateList(Location startLocation, Location endLocation, int alternateNum);
        IQueryable<Location> GiveLocations();
        IQueryable<Location> SearchLocation(string name);
        ValueTask<Location> AddNewLocationAsync(Location location);
        Location FindLocation(string locationName);
        Location FindLocationByDistance(double distance);
        //object PrintLocationsToConsole();
    }
}
